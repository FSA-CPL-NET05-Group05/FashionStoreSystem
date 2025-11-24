import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
// import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin.component.html',
})
export class AdminComponent {
  // constructor(private authService: AuthService) {}

  // logout() {
  //   this.authService.logout();
  // }
}
