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

  currentPage = 1;
  itemsPerPage = 8;
  totalPages = 1;

  constructor(
    private orderService: OrderService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.orderService.getOrders(this.currentPage, this.itemsPerPage).subscribe({
      next: (orders) => {
        this.orders = orders;
        if (orders.length === this.itemsPerPage) {
          this.totalPages = this.currentPage + 1;
        } else {
          this.totalPages = this.currentPage;
        }
      },
      error: (err) => {
        this.toastr.error('Failed to load orders');
        console.error(err);
      },
    });
  }

  get paginatedOrders() {
    return this.orders;
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadOrders();
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
    this.orderService.getOrderDetails(order.id).subscribe({
      next: (orderDetail) => {
        this.selectedOrder = orderDetail;
        this.showOrderDetail = true;
      },
      error: (err) => {
        this.toastr.error('Failed to load order details');
        console.error(err);
      },
    });
  }

  closeOrderDetail() {
    this.showOrderDetail = false;
    this.selectedOrder = null;
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
    if (order.status === 'Completed') {
      this.toastr.warning('Cannot change status of completed order');
      return;
    }

    this.orderService.updateOrderStatus(order.id, order.status).subscribe({
      next: () => {
        this.toastr.success('Order status updated successfully');
        if (this.showOrderDetail && this.selectedOrder?.id === order.id) {
          this.selectedOrder.status = order.status;
        }
        this.loadOrders();
      },
      error: (err) => {
        this.toastr.error('Failed to update order status');
        console.error(err);
        this.loadOrders();
      },
    });
  }
}
