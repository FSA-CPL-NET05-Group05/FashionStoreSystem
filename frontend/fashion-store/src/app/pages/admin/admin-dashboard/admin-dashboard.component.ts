import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';

import { ProductService } from '../../../services/product.service';
import { OrderService } from '../../../services/order.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
})
export class AdminDashboardComponent implements OnInit {
  stats = { products: 0, orders: 0, revenue: 0 };
  recentOrders: any[] = [];

  constructor(
    private productService: ProductService,
    private orderService: OrderService
  ) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    forkJoin({
      products: this.productService.getProducts(),
      orders: this.orderService.getOrders(1, 1000), // Lấy nhiều orders để tính toán stats
    }).subscribe({
      next: ({ products, orders }) => {
        console.log('Dashboard data:', { products, orders });

        // Calculate stats
        this.stats.products = products?.length || 0;
        this.stats.orders = orders?.length || 0;
        this.stats.revenue =
          orders?.reduce((sum, o) => sum + (o.totalAmount || 0), 0) || 0;

        // Get 5 recent orders (already sorted by backend)
        this.recentOrders = orders?.slice(0, 5) || [];

        console.log('Calculated stats:', this.stats);
        console.log('Recent orders:', this.recentOrders);
      },
      error: (err) => {
        console.error('Failed to load admin dashboard', err);
        // Set default values on error
        this.stats = { products: 0, orders: 0, revenue: 0 };
        this.recentOrders = [];
      },
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }
}
