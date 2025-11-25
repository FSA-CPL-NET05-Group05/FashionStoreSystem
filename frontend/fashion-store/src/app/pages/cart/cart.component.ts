import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './cart.component.html',
})
export class CartComponent implements OnInit, OnDestroy {
  cartItems: any[] = [];
  subtotal = 0;
  showSuccessModal = false;

  private cartSub?: Subscription;

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

  ngOnDestroy() {
    this.cartSub?.unsubscribe();
  }

  loadCart() {
    this.cartSub = this.cartService.getCart().subscribe({
      next: (items) => {
        this.cartItems = items;
        this.calculateTotals();
      },
      error: () => this.toastr.error('Failed to load cart'),
    });
  }

  calculateTotals() {
    this.subtotal = this.cartItems.reduce(
      (sum, item) => sum + (item.price || 0) * item.quantity,
      0
    );
  }

  updateQuantity(item: any, newQuantity: number) {
    if (newQuantity < 1) return;
    this.cartService.updateCartItem(item.id, newQuantity).subscribe({
      next: () => {
        item.quantity = newQuantity;
        this.calculateTotals();
        this.toastr.success('Cart updated');
      },
      error: () => this.toastr.error('Failed to update cart'),
    });
  }

  removeItem(item: any) {
    if (!confirm('Remove this item?')) return;
    this.cartService.removeFromCart(item.id).subscribe({
      next: () => {
        this.cartItems = this.cartItems.filter((i) => i.id !== item.id);
        this.calculateTotals();
        this.toastr.success('Item removed');
      },
      error: () => this.toastr.error('Failed to remove item'),
    });
  }

  initiateCheckout() {
    const user = this.authService.currentUserValue;

    if (!user) {
      this.toastr.warning('Please login');
      this.router.navigate(['/login']);
      return;
    }

    const guestInfo = {
      name: user?.fullName || 'Guest',
      phone: (user as any)?.phone || '0000000000',
      email: (user as any)?.email || 'guest@example.com',
    };

    this.placeOrder(user.id?.toString(), guestInfo);
  }

  placeOrder(
    userId?: string,
    guestInfo?: { name: string; phone: string; email: string }
  ) {
    this.orderService.createOrder(this.cartItems, guestInfo, userId).subscribe({
      next: () => {
        this.cartItems = [];
        this.calculateTotals();
        this.showSuccessModal = true;
        this.toastr.success('Order placed successfully!');
      },
      error: (err) => {
        console.error(err);
        this.toastr.error('Failed to place order');
      },
    });
  }

  closeSuccessModal() {
    this.showSuccessModal = false;
    this.router.navigate(['/shop']);
  }
}
