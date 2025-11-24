import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { StockService } from '../../services/stock.service';
import { ProductService } from '../../services/product.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';
import { FeedbackService } from '../../services/feedback.service';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-detail',
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: '../product-detail/product-detail.component.html',
})
export class ProductDetailComponent implements OnInit {
  product: any = null;
  selectedImage = '';
  currentStock = 0;
  quantity = 1;
  canReview = false;
  hasReviewed = false;
  showReviewForm = false;

  selectedSize: any = null;
  selectedColor: any = null;

  availableSizes: any[] = [];
  availableColors: any[] = [];
  reviewForm = { rating: 5, comment: '' };

  feedbacks: any[] = [];
  averageRating = 0;

  route = inject(ActivatedRoute);
  stockService = inject(StockService);
  productService = inject(ProductService);
  toastr = inject(ToastrService);
  authService = inject(AuthService);
  feedbackService = inject(FeedbackService);
  cartService = inject(CartService);
  http = inject(HttpClient);

  private apiUrl = 'http://localhost:3000';

  ngOnInit(): void {
    window.scrollTo(0, 0);
    const id = +this.route.snapshot.params['id'];
    this.loadProductDetail(id);
    this.loadFeedbacks(id);
    this.checkReviewPermission(id);
  }

  loadProductDetail(id: number) {
    forkJoin({
      product: this.productService.getProduct(id),
      productSizes: this.productService.getProductSizes(id),
      sizes: this.productService.getSizes(),
      colors: this.productService.getColors(),
    }).subscribe({
      next: ({ product, productSizes, sizes, colors }) => {
        this.product = product;
        this.selectedImage = product.imageUrl;

        const uniqueSizeIds = [
          ...new Set(productSizes.map((ps: any) => ps.sizeId)),
        ];
        this.availableSizes = sizes.filter((s) => uniqueSizeIds.includes(s.id));

        const uniqueColorIds = [
          ...new Set(productSizes.map((ps: any) => ps.colorId)),
        ];
        this.availableColors = colors.filter((c) =>
          uniqueColorIds.includes(c.id)
        );

        if (this.availableSizes.length)
          this.selectedSize = this.availableSizes[0];
        if (this.availableColors.length)
          this.selectedColor = this.availableColors[0];

        this.updateStock();
      },
    });
  }

  loadFeedbacks(productId: number) {
    this.feedbackService.getFeedbacks(productId).subscribe({
      next: (feedbacks) => {
        this.feedbacks = feedbacks;

        if (feedbacks.length) {
          const sum = feedbacks.reduce(
            (acc: number, f: any) => acc + f.rating,
            0
          );
          this.averageRating = Math.round(sum / feedbacks.length);
        }

        if (this.authService.currentUserValue) {
          const userId = this.authService.currentUserValue.id;
          this.hasReviewed = feedbacks.some((f: any) => f.userId === userId);
        }
      },
      error: (err) => {
        console.error('Failed to load feedbacks:', err);
      },
    });
  }

  checkReviewPermission(productId: number) {
    if (!this.authService.isCustomer()) {
      this.canReview = false;
      return;
    }

    const userId = this.authService.currentUserValue.id;

    this.http
      .get<any[]>(
        `${this.apiUrl}/purchaseHistory?userId=${userId}&productId=${productId}`
      )
      .subscribe({
        next: (purchases) => {
          this.canReview = purchases.length > 0;
        },
        error: (err) => {
          console.error('Failed to check purchase history:', err);
          this.canReview = false;
        },
      });
  }

  selectSize(size: any) {
    this.selectedSize = size;
    this.updateStock();
  }

  selectColor(color: any) {
    this.selectedColor = color;
    this.updateStock();
  }

  updateStock() {
    if (!this.selectedSize || !this.selectedColor) return;

    this.stockService
      .getStock(this.product.id, this.selectedSize.id, this.selectedColor.id)
      .subscribe({
        next: (productSize) => {
          this.currentStock = productSize?.stock || 0;
          if (this.quantity > this.currentStock) {
            this.quantity = Math.max(1, this.currentStock);
          }
        },
        error: (err) => {
          console.error('Failed to get stock:', err);
          this.currentStock = 0;
        },
      });
  }

  increaseQuantity() {
    if (this.quantity < this.currentStock) {
      this.quantity++;
    } else {
      this.toastr.warning(
        `Only ${this.currentStock} items available`,
        'Stock Limit'
      );
    }
  }

  decreaseQuantity() {
    if (this.quantity > 1) this.quantity--;
  }

  addToCart() {
    if (this.isAdmin()) {
      this.toastr.warning('Admin cannot purchase products');
      return;
    }

    if (!this.selectedSize || !this.selectedColor) {
      this.toastr.warning('Please select size and color');
      return;
    }

    if (this.currentStock < this.quantity) {
      this.toastr.error('Not enough stock available');
      return;
    }

    const userId = this.authService.currentUserValue?.id;

    if (!userId) {
      this.toastr.warning('Please login to add items to cart');
      return;
    }

    this.cartService
      .addToCart({
        userId: userId,
        productId: this.product.id,
        sizeId: this.selectedSize.id,
        colorId: this.selectedColor.id,
        quantity: this.quantity,
      })
      .subscribe({
        next: () => {
          this.toastr.success('Added to cart successfully!');
          this.updateStock();
        },
        error: (err) => {
          this.toastr.error(err.message || 'Failed to add to cart');
        },
      });
  }

  submitReview(e: Event) {
    e.preventDefault();

    if (!this.authService.isCustomer()) {
      this.toastr.warning('Only customers can write reviews');
      return;
    }

    if (!this.canReview) {
      this.toastr.warning('You need to purchase this product first');
      return;
    }

    if (this.hasReviewed) {
      this.toastr.warning('You have already reviewed this product');
      return;
    }

    if (!this.reviewForm.comment.trim()) {
      this.toastr.warning('Please write your review');
      return;
    }

    this.feedbackService
      .createFeedback({
        userId: this.authService.currentUserValue.id,
        productId: this.product.id,
        rating: this.reviewForm.rating,
        comment: this.reviewForm.comment,
      })
      .subscribe({
        next: () => {
          this.toastr.success('Review submitted successfully!');
          this.showReviewForm = false;
          this.reviewForm = { rating: 5, comment: '' };
          this.loadFeedbacks(this.product.id);
          this.hasReviewed = true;
        },
        error: (err) => {
          this.toastr.error(err.message || 'Failed to submit review');
        },
      });
  }

  formatDate(date: any): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  isAdmin(): boolean {
    const user = this.authService.currentUserValue;
    return user?.role === 'admin';
  }
}
