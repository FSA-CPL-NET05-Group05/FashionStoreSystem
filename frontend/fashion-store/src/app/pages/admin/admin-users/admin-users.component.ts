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

  constructor(
    private userService: UserService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers().subscribe((users) => {
      this.users = users;
    });
  }

  banUser(id: string) {
    this.userService.banUser(id).subscribe({
      next: () => {
        this.toastr.success('User banned');
        this.loadUsers();
      },
    });
  }

  unbanUser(id: string) {
    this.userService.unbanUser(id).subscribe({
      next: () => {
        this.toastr.success('User unbanned');
        this.loadUsers();
      },
    });
  }
}
