import { Component, inject, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-shop',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: '../shop/shop.component.html',
})
export class ShopComponent implements OnInit {
  products: any[] = [];
  filteredProducts: any[] = [];
  categories: any[] = [];

  searchTerm = '';
  sortBy = 'default';
  filters = { categoryId: '' };

  // Pagination
  currentPage = 1;
  itemsPerPage = 8;

  productService = inject(ProductService);

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
  }

  loadCategories() {
    this.productService.getCategories().subscribe((categories) => {
      this.categories = categories;
    });
  }

  loadProducts() {
    this.productService.getProducts().subscribe((products) => {
      this.products = products;
      this.applyFilters();
    });
  }

  applyFilters() {
    let filtered = [...this.products];

    // Search
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter((product) =>
        product.name.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }

    // Category Filter
    if (this.filters.categoryId) {
      filtered = filtered.filter(
        (product) => product.categoryId === +this.filters.categoryId
      );
    }

    this.filteredProducts = filtered;
    this.applySorting();
    this.currentPage = 1;
  }

  applySorting() {
    switch (this.sortBy) {
      case 'price-low':
        this.filteredProducts.sort((a, b) => a.price - b.price);
        break;
      case 'price-high':
        this.filteredProducts.sort((a, b) => b.price - a.price);
        break;
    }
  }

  clearFilters() {
    this.searchTerm = '';
    this.sortBy = 'default';
    this.filters = { categoryId: '' };
    this.filteredProducts = [...this.products];
    this.currentPage = 1;
  }

  // Pagination
  getDisplayedProducts() {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredProducts.slice(startIndex, endIndex);
  }

  getTotalPages() {
    return Math.ceil(this.filteredProducts.length / this.itemsPerPage);
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
