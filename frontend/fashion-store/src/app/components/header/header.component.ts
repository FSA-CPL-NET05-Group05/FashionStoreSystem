import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-header',
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: '../header/header.component.html',
})
export class HeaderComponent implements OnInit {
  currentUser: any = null;
  cartCount = 0;
  showMenu = false;
  showLoginModal = false;
  loginForm = { username: '', password: '' };

  authService = inject(AuthService);
  router = inject(Router);

  constructor(private toast: ToastrService) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe((user) => {
      this.currentUser = user;
      const userId = user ? user.id : 'guest';
    });
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
    this.authService
      .login(this.loginForm.username, this.loginForm.password)
      .subscribe({
        next: (user) => {
          this.showLoginModal = false;
          this.loginForm = { username: '', password: '' };
          this.toast.success('Login successful');
          if (user.role === 'admin') {
            this.router.navigate(['/admin']);
          }
        },
        error: (err) => {
          this.toast.error(err.message || 'Login failed');
        },
      });
  }

  logout() {
    this.authService.logout();
    this.showMenu = false;
    this.toast.info('Logged out successfully');
  }
}
