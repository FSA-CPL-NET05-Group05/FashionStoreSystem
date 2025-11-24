import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { forkJoin } from 'rxjs';
import { ProductService } from '../../../services/product.services';
import { AdminComponent } from '../admin.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './admin-dashboard.component.html',
})
export class AdminDashboardComponent implements OnInit {
  stats = { products: 0, orders: 0, revenue: 0, reviews: 0 };
  recentOrders: any[] = [];

  constructor(
    private productService: ProductService,

  ) {}

  ngOnInit() {
    // Chạy nhiều API song song
    forkJoin({
      products: this.productService.getProducts(),
     
    }).subscribe({
      next: ({ products,}) => {

        // Set thống kê
        this.stats.products = products.length;
      
      },
    });
  }
}
