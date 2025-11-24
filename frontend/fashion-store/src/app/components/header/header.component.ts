import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: '../header/header.component.html',
})
export class HeaderComponent implements OnInit, OnDestroy {
  currentUser: any = null;
  cartCount = 0;
  showMenu = false;
  showLoginModal = false;
  loginForm = { username: '', password: '' };

  private cartCountSubscription?: Subscription;
  private authSubscription?: Subscription;

  authService = inject(AuthService);
  cartService = inject(CartService);
  router = inject(Router);
  toast = inject(ToastrService);

  ngOnInit(): void {
    this.authSubscription = this.authService.currentUser$.subscribe((user) => {
      this.currentUser = user;

      if (user && user.id) {
        this.cartService.initializeCart(user.id);
      } else {
        this.cartCount = 0;
      }
    });

    this.cartCountSubscription = this.cartService.cartCount$.subscribe({
      next: (count) => {
        this.cartCount = count;
        console.log('Cart count updated in header:', count);
      },
      error: (err) => {
        console.error('Error subscribing to cart count:', err);
        this.cartCount = 0;
      },
    });
  }

  ngOnDestroy(): void {
    if (this.cartCountSubscription) {
      this.cartCountSubscription.unsubscribe();
    }
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  getUserInitials() {
    return (
      this.currentUser?.fullName
        ?.split(' ')
        .map((n: string) => n[0])
        .join('')
        .toUpperCase() || 'U'
    );
  }

  login(e: Event) {
    e.preventDefault();

    if (!this.loginForm.username || !this.loginForm.password) {
      this.toast.warning('Please enter username and password');
      return;
    }

    this.authService
      .login(this.loginForm.username, this.loginForm.password)
      .subscribe({
        next: (response) => {
          this.showLoginModal = false;
          this.loginForm = { username: '', password: '' };
          this.toast.success('Login successful');

          this.cartService.initializeCart(response.user.id);

          if (response.user.role === 'admin') {
            this.router.navigate(['/admin']);
          } else {
            this.router.navigate(['/']);
          }
        },
        error: (err) => {
          this.toast.error(err.message || 'Invalid username or password');
        },
      });
  }

  logout() {
    this.authService.logout();
    this.showMenu = false;
    this.cartCount = 0;
    this.toast.info('Logged out successfully');
    this.router.navigate(['/']);
  }
}
