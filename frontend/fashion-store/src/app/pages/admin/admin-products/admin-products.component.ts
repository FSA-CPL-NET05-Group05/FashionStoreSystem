import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ToastrService } from 'ngx-toastr';
import { ProductService } from '../../../services/product.services';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products.component.html',
})
export class AdminProductsComponent implements OnInit {
  products: any[] = [];
  categories: any[] = [];
  sizes: any[] = [];
  colors: any[] = [];

  showAddProductModal = false;
  showEditProductModal = false;
  showDeleteModal = false;
  showStockModal = false;

  newProduct: any = {
    name: '',
    categoryId: '',
    price: 0,
    description: '',
    imageUrl: '',
  };

  selectedProduct: any = null;
  stockData: any[] = [];

  constructor(private productService: ProductService, private toastr: ToastrService) {}

  ngOnInit() {
    this.loadInitialData();
  }

  loadInitialData() {
    this.productService.getProducts().subscribe((res) => {
      this.products = res;
    });
    this.productService.getCategories().subscribe((res) => {
      this.categories = res;
    });
    this.productService.getSizes().subscribe((res) => {
      this.sizes = res;
    });
    this.productService.getColors().subscribe((res) => {
      this.colors = res;
    });
  }

  openAddModal() {
    this.showAddProductModal = true;
  }

  closeAddModal() {
    this.showAddProductModal = false;
  }

  saveProduct() {
    this.productService.createProduct(this.newProduct).subscribe({
      next: () => {
        this.toastr.success('Product added!');
        this.loadInitialData();
        this.closeAddModal();
      },
      error: () => this.toastr.error('Failed to add product'),
    });
  }

  openEditModal(product: any) {
    this.selectedProduct = { ...product };
    this.showEditProductModal = true;
  }

  closeEditModal() {
    this.showEditProductModal = false;
  }

  updateProduct() {
    this.productService.updateProduct(this.selectedProduct.id, this.selectedProduct).subscribe({
      next: () => {
        this.toastr.success('Product updated!');
        this.loadInitialData();
        this.closeEditModal();
      },
      error: () => this.toastr.error('Update failed'),
    });
  }

  openDeleteModal(product: any) {
    this.selectedProduct = product;
    this.showDeleteModal = true;
  }

  confirmDelete() {
    this.productService.deleteProduct(this.selectedProduct.id).subscribe({
      next: () => {
        this.toastr.success('Product deleted');
        this.loadInitialData();
        this.showDeleteModal = false;
      },
      error: () => this.toastr.error('Delete failed'),
    });
  }

  closeStockModal() {
    this.showStockModal = false;
  }
}
