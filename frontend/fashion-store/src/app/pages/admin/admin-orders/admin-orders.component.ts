import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ToastrService } from 'ngx-toastr';
import { OrderService } from '../../../services/order.service';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-orders.component.html',
})
export class AdminOrdersComponent implements OnInit {
  orders: any[] = [];
  showOrderDetail = false;
  selectedOrder: any = null;
  orderDetails: any[] = [];

  // Pagination
  currentPage = 1;
  itemsPerPage = 7;
  totalPages = 0;

  constructor(
    private orderService: OrderService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.orderService.getOrders().subscribe((orders) => {
      this.orders = orders;
      this.calculateTotalPages();
    });
  }

  // Pagination methods
  get paginatedOrders() {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.orders.slice(start, end);
  }

  calculateTotalPages() {
    this.totalPages = Math.ceil(this.orders.length / this.itemsPerPage);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  get pageNumbers(): number[] {
    const pages = [];
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(this.totalPages, start + maxVisible - 1);

    if (end - start < maxVisible - 1) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  viewOrderDetail(order: any) {
    this.selectedOrder = order;
    this.orderService.getOrderDetails(order.id).subscribe((details) => {
      this.orderDetails = details;
      this.showOrderDetail = true;
    });
  }

  closeOrderDetail() {
    this.showOrderDetail = false;
    this.selectedOrder = null;
    this.orderDetails = [];
  }

  formatOrderDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  updateStatus(order: any) {
    this.orderService.updateOrderStatus(order.id, order.status).subscribe({
      next: () => this.toastr.success('Status updated'),
      error: () => this.toastr.error('Failed to update'),
    });
  }
}
