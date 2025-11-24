import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: `../cart/cart.component.html`,
})
export class CartComponent implements OnInit, OnDestroy {
  cartItems: any[] = [];
  subtotal = 0;
  showSuccessModal = false;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    public authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadCart();
  }

  ngOnDestroy() {}

  loadCart() {
    const userId = this.authService.currentUserValue?.id;

    if (!userId) return;

    this.cartService.getCartWithDetails(userId).subscribe({
      next: (items) => {
        this.cartItems = items;
        this.calculateTotals();
      },
    });
  }

  calculateTotals() {
    this.subtotal = this.cartItems.reduce(
      (sum, item) => sum + (item.product?.price || 0) * item.quantity,
      0
    );
  }

  updateQuantity(item: any, newQuantity: number) {
    if (newQuantity < 1) return;

    this.cartService
      .updateCartItem(item.id, newQuantity, item.quantity, item)
      .subscribe({
        next: () => {
          item.quantity = newQuantity;
          this.calculateTotals();
          this.toastr.success('Cart updated');
        },
        error: (err) => this.toastr.error(err.message),
      });
  }

  removeItem(item: any) {
    if (!confirm('Remove this item from cart?')) return;

    this.cartService.removeFromCart(item).subscribe({
      next: (response) => {
        this.cartItems = this.cartItems.filter((i) => i.id !== item.id);
        this.calculateTotals();
        this.toastr.success('Item removed from cart');
      },
      error: (err) => {
        console.error('Error removing item:', err);
        this.toastr.error('Failed to remove item');
        this.loadCart();
      },
    });
  }

  initiateCheckout() {
    const user = this.authService.currentUserValue;

    if (!user) {
      this.toastr.warning('Please login to place order');
      this.router.navigate(['/login']);
      return;
    }

    this.placeOrder(user.id);
  }

  placeOrder(userId: string) {
    this.orderService.createOrder(this.cartItems, null, userId).subscribe({
      next: () => {
        this.cartService.clearCartAfterOrder(userId).subscribe(() => {
          this.cartItems = [];
          this.calculateTotals();
          this.showSuccessModal = true;
          this.toastr.success('Order placed successfully!');
        });
      },
      error: () => this.toastr.error('Failed to place order'),
    });
  }

  closeSuccessModal() {
    this.showSuccessModal = false;
    this.router.navigate(['/shop']);
  }
}
