import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';

import { ProductService } from '../../../services/product.service';
import { OrderService } from '../../../services/order.service';
import { UserService } from '../../../services/user.service';
import { FeedbackService } from '../../../services/feedback.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
})
export class AdminDashboardComponent implements OnInit {
  stats = { products: 0, orders: 0, revenue: 0, reviews: 0 };
  recentOrders: any[] = [];

  constructor(
    private productService: ProductService,
    private orderService: OrderService,
    private userService: UserService,
    private feedbackService: FeedbackService
  ) {}

  ngOnInit() {
    forkJoin({
      products: this.productService.getProducts(),
      orders: this.orderService.getOrders(),
      feedbacks: this.feedbackService.getAllFeedbacks(), // dùng phương thức mới
    }).subscribe({
      next: ({ products, orders, feedbacks }) => {
        this.stats.products = products.length;
        this.stats.orders = orders.length;
        this.stats.revenue = orders.reduce((sum, o) => sum + o.totalAmount, 0);
        this.stats.reviews = feedbacks.length;
        this.recentOrders = orders.slice(0, 5);
      },
      error: (err) => console.error('Failed to load admin dashboard', err),
    });
  }
}
