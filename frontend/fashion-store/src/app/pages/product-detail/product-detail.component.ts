import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { FeedbackService } from '../../services/feedback.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: './product-detail.component.html',
})
export class ProductDetailComponent implements OnInit {
  // ====== Product & Variants ======
  product: any = null;
  selectedImage = '';
  currentStock = 0;
  quantity = 1;
  selectedSize: any = null;
  selectedColor: any = null;
  availableSizes: any[] = [];
  availableColors: any[] = [];

  // ====== Feedback ======
  feedbacks: any[] = [];
  averageRating = 0;
  showReviewForm = false;
  reviewForm = { rating: 5, comment: '' };

  // ====== inject ======
  route = inject(ActivatedRoute);
  productService = inject(ProductService);
  cartService = inject(CartService);
  authService = inject(AuthService);
  feedbackService = inject(FeedbackService);
  toastr = inject(ToastrService);

  ngOnInit(): void {
    window.scrollTo(0, 0);
    const id = +this.route.snapshot.params['id'];
    this.loadProductDetail(id);
    this.loadFeedbacks(id);
  }

  // ================= Product & Variants =================
  loadProductDetail(id: number) {
    this.productService.getProduct(id).subscribe({
      next: (product) => {
        this.product = product;
        this.selectedImage = product.images?.[0] || product.imageUrl;

        const sizeMap = new Map<number, any>();
        const colorMap = new Map<number, any>();

        product.variants.forEach((v: any) => {
          if (!sizeMap.has(v.sizeId))
            sizeMap.set(v.sizeId, { id: v.sizeId, name: v.sizeName });
          if (!colorMap.has(v.colorId))
            colorMap.set(v.colorId, {
              id: v.colorId,
              name: v.colorName,
              code: v.colorCode,
            });
        });

        this.availableSizes = Array.from(sizeMap.values());
        this.availableColors = Array.from(colorMap.values());

        this.selectedSize = this.availableSizes[0] || null;
        this.selectedColor = this.availableColors[0] || null;

        this.updateStock();
      },
      error: () => this.toastr.error('Failed to load product'),
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
    if (!this.product || !this.selectedSize || !this.selectedColor) return;
    const variant = this.product.variants.find(
      (v: any) =>
        v.sizeId === this.selectedSize.id && v.colorId === this.selectedColor.id
    );
    this.currentStock = variant?.stock || 0;
    if (this.quantity > this.currentStock)
      this.quantity = Math.max(1, this.currentStock);
  }

  increaseQuantity() {
    if (this.quantity < this.currentStock) this.quantity++;
    else this.toastr.warning(`Only ${this.currentStock} items available`);
  }
  decreaseQuantity() {
    if (this.quantity > 1) this.quantity--;
  }

  addToCart() {
    const userId = this.authService.currentUserValue?.id?.toString();
    if (!userId) {
      this.toastr.warning('Please login');
      return;
    }
    this.cartService
      .addToCart({
        productId: this.product.id,
        sizeId: this.selectedSize.id,
        colorId: this.selectedColor.id,
        quantity: this.quantity,
      })
      .subscribe({
        next: () => this.toastr.success('Added to cart'),
        error: (err) =>
          this.toastr.error(err.message || 'Failed to add to cart'),
      });
  }

  // ================= Feedback =================
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
      },
      error: () => this.toastr.error('Failed to load feedbacks'),
    });
  }

  canReview(): boolean {
    return !!this.authService.currentUserValue;
  }

  submitReview(event: Event) {
    event.preventDefault();

    const userId = this.authService.currentUserValue?.id?.toString();
    if (!userId) {
      this.toastr.warning('Please login to submit review');
      return;
    }

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
        },
        error: (err: any) => {
          let msg = 'Failed to submit review';
          if (err.error) {
            if (typeof err.error === 'string') {
              msg = err.error;
            } else if (err.error.message) {
              msg = err.error.message;
            } else {
              msg = JSON.stringify(err.error);
            }
          } else if (err.message) {
            msg = err.message;
          }
          this.toastr.error(msg);
        },
      });
  }

  formatDate(dateStr: string) {
    return new Date(dateStr).toLocaleDateString();
  }
}
