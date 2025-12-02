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

  // Inject services (chỉ dùng 1 cách)
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private router = inject(Router);
  private toast = inject(ToastrService);

  ngOnInit(): void {
    this.authSubscription = this.authService.currentUser$.subscribe((user) => {
      this.currentUser = user;

      if (user) {
        // Lấy cart và cập nhật count
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
      next: (user: any) => {
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
      error: (error: any) => {
        console.error('❌ Login error:', error);
        
        if (error.status === 401) {
     
          const errorCode = error.error?.code;
          
          if (errorCode === 'ACCOUNT_LOCKED') {
            this.toast.error(
              'Your account has been locked. Please contact support for assistance.',
              'Account Locked',
              { timeOut: 6000 }
            );
          } else if (errorCode === 'INVALID_CREDENTIALS') {
            this.toast.error(
              'Invalid username or password. Please try again.',
              'Login Failed',
              { timeOut: 5000 }
            );
          } else {
            
            this.toast.error(
              error.error?.message || 'Invalid username or password',
              'Login Failed',
              { timeOut: 5000 }
            );
          }
        } else if (error.status === 400) {
          this.toast.error(error.error?.message || 'Invalid data');
        } else if (error.status === 0) {
          this.toast.error('Cannot connect to server');
        } else {
          this.toast.error(error.message || 'Login failed');
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