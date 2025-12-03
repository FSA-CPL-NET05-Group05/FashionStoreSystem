import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-users.component.html',
})
export class AdminUsersComponent implements OnInit {
  users: any[] = [];
  currentPage = 1;
  itemsPerPage = 4;
  totalPages = 0;

  constructor(
    private userService: UserService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.currentPage, this.itemsPerPage).subscribe({
      next: (res) => {
        this.users = res.items;
        this.totalPages = res.totalPages;
        console.log('Users loaded:', this.users);
      },
      error: (err) => {
        console.error('Error response:', err);
        this.toastr.error('Failed to load users');
      },
    });
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadUsers();
    }
  }

  get pageNumbers(): number[] {
    const pages = [];
    for (let i = 1; i <= this.totalPages; i++) pages.push(i);
    return pages;
  }

  trackById(index: number, user: any) {
    return user.id;
  }

  banUser(id: string) {
    this.userService.banUser(id).subscribe({
      next: (res: any) => {
        this.toastr.success(res?.message || 'User banned');
        this.loadUsers();
      },
      error: (err: any) => {
        let msg = 'Failed to ban user';
        if (err.error) {
          if (typeof err.error === 'string') msg = err.error;
          else if (err.error.message) msg = err.error.message;
          else msg = JSON.stringify(err.error);
        } else if (err.message) msg = err.message;
        this.toastr.error(msg);
      },
    });
  }

  unbanUser(id: string) {
    this.userService.unbanUser(id).subscribe({
      next: (res: any) => {
        this.toastr.success(res?.message || 'User unbanned');
        this.loadUsers();
      },
      error: (err: any) => {
        let msg = 'Failed to unban user';
        if (err.error) {
          if (typeof err.error === 'string') msg = err.error;
          else if (err.error.message) msg = err.error.message;
          else msg = JSON.stringify(err.error);
        } else if (err.message) msg = err.message;
        this.toastr.error(msg);
      },
    });
  }
}
