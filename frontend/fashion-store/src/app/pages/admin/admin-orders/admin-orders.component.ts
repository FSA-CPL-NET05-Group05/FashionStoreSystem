import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ToastrService } from 'ngx-toastr';
import { OrderService } from '../../../services/order.service';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-orders.component.html'
})
export class AdminOrdersComponent implements OnInit {
  orders: any[] = [];
  showOrderDetail = false;
  selectedOrder: any = null;
  orderDetails: any[] = [];

  constructor(
    private orderService: OrderService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.orderService.getOrders().subscribe((orders) => {
      this.orders = orders;
    });
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
