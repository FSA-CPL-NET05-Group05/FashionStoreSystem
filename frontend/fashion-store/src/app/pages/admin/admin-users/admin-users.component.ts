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
  isLoading = false;

  constructor(
    private userService: UserService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading = true;
    this.userService.getUsers(this.currentPage, this.itemsPerPage).subscribe({
      next: (res) => {
        this.users = res.items;
        this.totalPages = res.totalPages;
        this.isLoading = false;
        console.log('✅ Users loaded:', this.users);
      },
      error: (err) => {
        this.isLoading = false;
        console.error('❌ Error loading users:', err);
        // Interceptor đã xử lý toast, không cần hiển thị lại
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
    if (!confirm('Bạn có chắc muốn khóa tài khoản này?')) return;

    this.userService.banUser(id).subscribe({
      next: (res: any) => {
        this.toastr.success('Đã khóa tài khoản thành công', 'Thành công');
        this.loadUsers();
      },
      error: (err: any) => {
        console.error('❌ Error banning user:', err);
        // Interceptor đã xử lý lỗi 401, 403
        // Chỉ xử lý lỗi đặc thù ở đây nếu cần
        if (err.status === 400) {
          this.toastr.error(
            err.error?.message || 'Không thể khóa tài khoản',
            'Lỗi'
          );
        }
      },
    });
  }

  unbanUser(id: string) {
    if (!confirm('Bạn có chắc muốn mở khóa tài khoản này?')) return;

    this.userService.unbanUser(id).subscribe({
      next: (res: any) => {
        this.toastr.success('Đã mở khóa tài khoản thành công', 'Thành công');
        this.loadUsers();
      },
      error: (err: any) => {
        console.error('❌ Error unbanning user:', err);
        // Interceptor đã xử lý lỗi 401, 403
        // Chỉ xử lý lỗi đặc thù ở đây nếu cần
        if (err.status === 400) {
          this.toastr.error(
            err.error?.message || 'Không thể mở khóa tài khoản',
            'Lỗi'
          );
        }
      },
    });
  }
}