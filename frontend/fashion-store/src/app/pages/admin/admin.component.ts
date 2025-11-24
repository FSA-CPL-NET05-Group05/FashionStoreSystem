import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin.component.html',
})
export class AdminComponent {
  constructor(private authService: AuthService, private toast: ToastrService) {}

  logout() {
    this.authService.logout();
    this.toast.info('Logged out successfully');
  }
}
