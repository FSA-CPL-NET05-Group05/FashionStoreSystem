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
import { Component, inject, OnInit } from '@angular/core';

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
    const idParam = this.route.snapshot.params['id'];
    const id = typeof idParam === 'string' ? +idParam : idParam;
    this.loadProductDetail(id);
    this.loadFeedbacks(id);
    this.checkReviewPermission(id);
  }

  loadProductDetail(id: number): void {
    this.productService.getProduct(id).subscribe({
      next: (product) => {
        this.product = product;

        this.selectedImage = product.images?.[0] || product.imageUrl;

        const uniqueSizeMap = new Map<number, any>();
        const uniqueColorMap = new Map<number, any>();

        product.variants.forEach((v: any) => {
          if (!uniqueSizeMap.has(v.sizeId))
            uniqueSizeMap.set(v.sizeId, { id: v.sizeId, name: v.sizeName });
          if (!uniqueColorMap.has(v.colorId)) {
            uniqueColorMap.set(v.colorId, {
              id: v.colorId,
              name: v.colorName,
              code: v.colorCode,
            });
          }
        });

        this.availableSizes = Array.from(uniqueSizeMap.values());
        this.availableColors = Array.from(uniqueColorMap.values());

        if (this.availableSizes.length)
          this.selectedSize = this.availableSizes[0];
        if (this.availableColors.length)
          this.selectedColor = this.availableColors[0];

        this.updateStock();
      },
      error: (err) => console.error('Failed to load product detail', err),
    });
  }

  loadFeedbacks(productId: number): void {
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

        const currentUser = this.authService.currentUserValue;
        if (currentUser) {
          const userIdStr = currentUser.id.toString();
          this.hasReviewed = feedbacks.some(
            (f: any) => f.userId.toString() === userIdStr
          );
        }
      },
      error: (err) => console.error('Failed to load feedbacks:', err),
    });
  }

  checkReviewPermission(productId: number): void {
    if (!this.authService.isCustomer()) {
      this.canReview = false;
      return;
    }

    const userId = this.authService.currentUserValue?.id?.toString();
    if (!userId) {
      this.canReview = false;
      return;
    }

    this.http
      .get<any[]>(
        `${this.apiUrl}/purchaseHistory?userId=${userId}&productId=${productId}`
      )
      .subscribe({
        next: (purchases) => (this.canReview = purchases.length > 0),
        error: (err) => {
          console.error('Failed to check purchase history:', err);
          this.canReview = false;
        },
      });
  }

  selectSize(size: any): void {
    this.selectedSize = size;
    this.updateStock();
  }

  selectColor(color: any): void {
    this.selectedColor = color;
    this.updateStock();
  }

  updateStock(): void {
    if (!this.selectedSize || !this.selectedColor) return;

    const variant = this.product.variants.find(
      (v: any) =>
        v.sizeId === this.selectedSize.id && v.colorId === this.selectedColor.id
    );

    this.currentStock = variant?.stock || 0;
    if (this.quantity > this.currentStock)
      this.quantity = Math.max(1, this.currentStock);
  }

  increaseQuantity(): void {
    if (this.quantity < this.currentStock) this.quantity++;
    else
      this.toastr.warning(
        `Only ${this.currentStock} items available`,
        'Stock Limit'
      );
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) this.quantity--;
  }

  addToCart(): void {
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

    const userId = this.authService.currentUserValue?.id?.toString();
    if (!userId) {
      this.toastr.warning('Please login to add items to cart');
      return;
    }

    this.cartService
      .addToCart({
        userId,
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
        error: (err) =>
          this.toastr.error(err.message || 'Failed to add to cart'),
      });
  }

  submitReview(e: Event): void {
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

    const userId = this.authService.currentUserValue?.id?.toString();
    if (!userId) return;

    this.feedbackService
      .createFeedback({
        userId,
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
        error: (err) =>
          this.toastr.error(err.message || 'Failed to submit review'),
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
    return this.authService.currentUserValue?.role === 'admin';
  }
}
