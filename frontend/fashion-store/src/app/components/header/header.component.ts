import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { User } from '../../models/models';

@Component({
  selector: 'app-header',
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit, OnDestroy {
  currentUser: User | null = null;
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

      if (user) {
        this.cartService.getCart().subscribe();
      } else {
        this.cartCount = 0;
      }
    });

    this.cartCountSubscription = this.cartService.cartCount$.subscribe({
      next: (count) => (this.cartCount = count),
      error: () => (this.cartCount = 0),
    });
  }

  ngOnDestroy(): void {
    this.authSubscription?.unsubscribe();
    this.cartCountSubscription?.unsubscribe();
  }

  getUserInitials(): string {
    return (
      this.currentUser?.fullName
        ?.split(' ')
        .map((n) => n[0])
        .join('')
        .toUpperCase() || 'U'
    );
  }

  login(e: Event): void {
    e.preventDefault();

    if (!this.loginForm.username || !this.loginForm.password) {
      this.toast.warning('Please enter username and password');
      return;
    }

    this.authService
      .login(this.loginForm.username, this.loginForm.password)
      .subscribe({
        next: (user) => {
          if (!user) {
            this.toast.error('Invalid username or password');
            return;
          }

          this.showLoginModal = false;
          this.loginForm = { username: '', password: '' };
          this.toast.success('Login successful');

          this.cartService.getCart().subscribe();

          if (user.role === 'Admin') this.router.navigate(['/admin']);
          else this.router.navigate(['/']);
        },
        error: (err) => {
          console.error('Login error:', err);

          if (err.status === 401) {
            this.toast.error(
              'Login failed. Please check your credentials or contact support if your account has been banned.',
              'Login Failed',
              {
                timeOut: 6000,
                closeButton: true,
              }
            );
          } else {
            this.toast.error('Login failed. Please try again.', 'Error');
          }
        },
      });
  }

  logout(): void {
    this.authService.logout();
    this.showMenu = false;
    this.cartCount = 0;
    this.toast.info('Logged out successfully');
    this.router.navigate(['/']);
  }
}
