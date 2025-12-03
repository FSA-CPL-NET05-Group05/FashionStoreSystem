import { ProductService } from '../../services/product.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Component, inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-shop',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: '../shop/shop.component.html',
})
export class ShopComponent implements OnInit {
  products: any[] = [];
  categories: any[] = [];

  searchTerm = '';
  sortBy = 'default';
  filters = { categoryId: '' };

  currentPage = 1;
  pageSize = 8;
  totalCount = 0;
  totalPages = 0;

  allProducts: any[] = [];
  isLoading = false;

  productService = inject(ProductService);

  ngOnInit(): void {
    this.loadCategories();
    this.loadAllProducts();
  }

  loadCategories() {
    this.productService.getCategories().subscribe((categories) => {
      this.categories = categories;
    });
  }

  loadAllProducts() {
    this.isLoading = true;

    this.productService.getProducts().subscribe({
      next: (products) => {
        this.allProducts = products;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load products:', err);
        this.isLoading = false;
      },
    });
  }

  applyFilters() {
    let filtered = [...this.allProducts];

    // Category Filter
    if (this.filters.categoryId) {
      filtered = filtered.filter(
        (product) => product.categoryId === +this.filters.categoryId
      );
    }

    // Search Filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter((product) =>
        product.name.toLowerCase().includes(term)
      );
    }

    // Apply Sorting
    this.applySorting(filtered);

    // Update products và pagination
    this.products = filtered;
    this.totalCount = filtered.length;
    this.totalPages = Math.ceil(this.totalCount / this.pageSize);

    // Reset về page 1 khi filter
    this.currentPage = 1;
  }

  applySorting(products: any[]) {
    switch (this.sortBy) {
      case 'price-low':
        products.sort((a, b) => a.price - b.price);
        break;
      case 'price-high':
        products.sort((a, b) => b.price - a.price);
        break;
      default:
        break;
    }
  }

  clearFilters() {
    this.searchTerm = '';
    this.sortBy = 'default';
    this.filters = { categoryId: '' };
    this.currentPage = 1;
    this.applyFilters();
  }

  getDisplayedProducts() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    return this.products.slice(startIndex, endIndex);
  }

  getTotalPages() {
    return this.totalPages;
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  getPageNumbers() {
    const total = this.getTotalPages();
    const pages = [];
    for (let i = 1; i <= total; i++) {
      pages.push(i);
    }
    return pages;
  }

  goToPage(page: number) {
    this.currentPage = page;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  nextPage() {
    if (this.currentPage < this.getTotalPages()) {
      this.currentPage++;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}
