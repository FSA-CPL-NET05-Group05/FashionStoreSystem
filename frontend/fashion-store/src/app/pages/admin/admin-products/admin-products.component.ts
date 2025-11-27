import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ToastrService } from 'ngx-toastr';
import { catchError, forkJoin, of } from 'rxjs';
import { ProductService } from '../../../services/product.service';

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
  currentProductStocks: any[] = [];

  // Pagination
  currentPage = 1;
  pageSize = 4;
  totalPages = 0;
  totalCount = 0;

  showProductModal = false;
  showStockModal = false;
  isEditMode = false;
  selectedProduct: any = null;

  productForm: any = {
    name: '',
    description: '',
    price: 0,
    categoryId: '',
    imageUrl: '',
    additionalImagesText: '',
  };

  stockForm: any = {
    sizeId: '',
    colorId: '',
    stock: 0,
  };

  validationErrors: any = {
    name: '',
    imageUrl: '',
    price: '',
    description: '',
    categoryId: '',
  };

  constructor(
    private productService: ProductService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    forkJoin({
      productsResponse: this.productService.getAdminProducts(
        this.currentPage,
        this.pageSize
      ),
      categories: this.productService.getCategories(),
    }).subscribe({
      next: ({ productsResponse, categories }) => {
        this.products = productsResponse.items || [];
        this.totalCount = productsResponse.totalCount || 0;
        this.totalPages = productsResponse.totalPages || 0;
        this.currentPage = productsResponse.page || 1;
        this.categories = categories;

        if (this.products.length > 0) {
          this.loadSizesAndColorsFromFirstProduct();
        }
      },
      error: (err) => {
        console.error('Failed to load data:', err);
        this.toastr.error('Failed to load products');
      },
    });
  }

  loadSizesAndColorsFromFirstProduct() {
    const firstProduct = this.products[0];
    if (firstProduct) {
      this.productService.getProductStocks(firstProduct.id).subscribe({
        next: (stocks) => {
          this.extractSizesAndColorsFromStocks(stocks);
        },
        error: () => {
          console.warn('Could not load sizes/colors from product stocks');
          this.sizes = [
            { id: 1, name: 'S' },
            { id: 2, name: 'M' },
            { id: 3, name: 'L' },
            { id: 4, name: 'XL' },
            { id: 5, name: 'XXL' },
          ];
          this.colors = [
            { id: 1, name: 'Đen', code: '#000000' },
            { id: 2, name: 'Trắng', code: '#FFFFFF' },
            { id: 3, name: 'Xám', code: '#808080' },
            { id: 4, name: 'Đỏ', code: '#FF0000' },
            { id: 5, name: 'Xanh Dương', code: '#0000FF' },
          ];
        },
      });
    }
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadData();
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

  getCategoryName(id: number): string {
    return this.categories.find((c) => c.id === id)?.name || '-';
  }

  getSizeName(id: number): string {
    return this.sizes.find((s) => s.id === id)?.name || '-';
  }

  getColorName(id: number): string {
    return this.colors.find((c) => c.id === id)?.name || '-';
  }

  getColorCode(id: number): string {
    return this.colors.find((c) => c.id === id)?.code || '#000000';
  }

  openAddModal() {
    this.isEditMode = false;
    this.productForm = {
      name: '',
      description: '',
      price: 0,
      categoryId: '',
      imageUrl: '',
      additionalImagesText: '',
    };
    this.validationErrors = {
      name: '',
      imageUrl: '',
      price: '',
      description: '',
      categoryId: '',
    };
    this.showProductModal = true;
  }

  editProduct(product: any) {
    this.isEditMode = true;
    this.productForm = {
      id: product.id,
      name: product.name,
      description: product.description,
      price: product.price,
      categoryId: product.categoryId,
      imageUrl: product.imageUrl,
      additionalImagesText: product.images?.slice(1).join(', ') || '',
    };
    this.validationErrors = {
      name: '',
      imageUrl: '',
      price: '',
      description: '',
      categoryId: '',
    };
    this.showProductModal = true;
  }

  saveProduct(e: Event) {
    e.preventDefault();

    if (!this.validateProduct(this.productForm)) {
      this.toastr.warning('Please fix all validation errors');
      return;
    }

    const data: any = {
      name: this.productForm.name,
      description: this.productForm.description,
      price: +this.productForm.price,
      categoryId: +this.productForm.categoryId,
      imageUrl: this.productForm.imageUrl,
      additionalImages: this.productForm.additionalImagesText
        ? this.productForm.additionalImagesText
            .split(',')
            .map((url: string) => url.trim())
            .filter((url: string) => url.length > 0)
        : [],
    };

    if (this.isEditMode) {
      this.productService.updateProduct(this.productForm.id, data).subscribe({
        next: () => {
          this.toastr.success('Product updated!');
          this.closeProductModal();
          this.loadData();
        },
        error: (err) => {
          console.error('Update error:', err);
          this.toastr.error('Failed to update product');
        },
      });
    } else {
      this.productService.createProduct(data).subscribe({
        next: () => {
          this.toastr.success('Product added!');
          this.closeProductModal();
          this.loadData();
        },
        error: (err) => {
          console.error('Create error:', err);
          this.toastr.error('Failed to add product');
        },
      });
    }
  }

  deleteProduct(id: number) {
    if (
      !confirm('Delete this product? This will remove all related stock data.')
    )
      return;

    this.productService.deleteProduct(id).subscribe({
      next: () => {
        this.toastr.success('Product deleted!');
        this.loadData();
      },
      error: (err) => {
        console.error('Delete error:', err);
        this.toastr.error('Failed to delete product');
      },
    });
  }

  manageStock(product: any) {
    this.selectedProduct = product;
    this.productService.getProductStocks(product.id).subscribe({
      next: (stocks) => {
        this.currentProductStocks = stocks;
        this.extractSizesAndColorsFromStocks(stocks);
        this.showStockModal = true;
        this.stockForm = { sizeId: '', colorId: '', stock: 0 };
      },
      error: (err) => {
        console.error('Load stock error:', err);
        this.toastr.error('Failed to load stock data');
      },
    });
  }

  extractSizesAndColorsFromStocks(stocks: any[]) {
    const sizeMap = new Map<number, any>();
    const colorMap = new Map<number, any>();

    stocks.forEach((stock) => {
      if (!sizeMap.has(stock.sizeId)) {
        sizeMap.set(stock.sizeId, { id: stock.sizeId, name: stock.sizeName });
      }
      if (!colorMap.has(stock.colorId)) {
        colorMap.set(stock.colorId, {
          id: stock.colorId,
          name: stock.colorName,
          code: this.getDefaultColorCode(stock.colorName),
        });
      }
    });

    const existingSizeIds = new Set(this.sizes.map((s) => s.id));
    const existingColorIds = new Set(this.colors.map((c) => c.id));

    Array.from(sizeMap.values()).forEach((size) => {
      if (!existingSizeIds.has(size.id)) {
        this.sizes.push(size);
      }
    });

    Array.from(colorMap.values()).forEach((color) => {
      if (!existingColorIds.has(color.id)) {
        this.colors.push(color);
      }
    });

    this.sizes.sort((a, b) => {
      const sizeOrder = ['XS', 'S', 'M', 'L', 'XL', 'XXL', 'XXXL'];
      const aIndex = sizeOrder.indexOf(a.name.toUpperCase());
      const bIndex = sizeOrder.indexOf(b.name.toUpperCase());
      if (aIndex === -1 && bIndex === -1) return a.id - b.id;
      if (aIndex === -1) return 1;
      if (bIndex === -1) return -1;
      return aIndex - bIndex;
    });
  }

  getDefaultColorCode(colorName: string): string {
    const colorMap: { [key: string]: string } = {
      đen: '#000000',
      black: '#000000',
      trắng: '#FFFFFF',
      white: '#FFFFFF',
      xám: '#808080',
      gray: '#808080',
      grey: '#808080',
      đỏ: '#FF0000',
      red: '#FF0000',
      'xanh dương': '#0000FF',
      xanh: '#0000FF',
      blue: '#0000FF',
      'xanh lá': '#00FF00',
      green: '#00FF00',
      vàng: '#FFFF00',
      yellow: '#FFFF00',
      cam: '#FFA500',
      orange: '#FFA500',
      hồng: '#FFC0CB',
      pink: '#FFC0CB',
      tím: '#800080',
      purple: '#800080',
      nâu: '#8B4513',
      brown: '#8B4513',
    };

    const lowerName = colorName.toLowerCase();
    return colorMap[lowerName] || '#808080';
  }

  addStock() {
    if (
      !this.stockForm.sizeId ||
      !this.stockForm.colorId ||
      this.stockForm.stock < 0
    ) {
      this.toastr.warning('Please select size, color and enter valid stock');
      return;
    }

    const newStock = {
      productId: this.selectedProduct.id,
      sizeId: +this.stockForm.sizeId,
      colorId: +this.stockForm.colorId,
      stock: +this.stockForm.stock,
    };

    console.log('Adding stock:', newStock);

    this.productService
      .createProductStock(this.selectedProduct.id, newStock)
      .subscribe({
        next: (response) => {
          console.log('Add stock response:', response);
          this.toastr.success('Stock added!');

          // Reload lại stock list từ server để chắc chắn
          this.productService
            .getProductStocks(this.selectedProduct.id)
            .subscribe({
              next: (stocks) => {
                console.log('Reloaded stocks:', stocks);
                this.currentProductStocks = [...stocks];

                // Cập nhật totalStock trong products array
                this.updateProductTotalStock(this.selectedProduct.id, stocks);

                this.stockForm = { sizeId: '', colorId: '', stock: 0 };
                this.cdr.detectChanges();
              },
              error: (err) => {
                console.error('Failed to reload stocks:', err);
              },
            });
        },
        error: (err) => {
          console.error('Add stock error:', err);
          if (err.error?.message) {
            this.toastr.error(err.error.message);
          } else {
            this.toastr.error('Failed to add stock');
          }
        },
      });
  }

  updateStock(stock: any, newStockValue: number) {
    if (newStockValue < 0) {
      this.toastr.warning('Stock cannot be negative');
      return;
    }

    console.log('Updating stock:', stock.id, 'to:', newStockValue);

    const oldValue = stock.stock;

    this.productService
      .updateProductStock(this.selectedProduct.id, stock.id, newStockValue)
      .subscribe({
        next: (response) => {
          console.log('Update stock response:', response);

          // Tìm và cập nhật trong array
          const index = this.currentProductStocks.findIndex(
            (s) => s.id === stock.id
          );
          if (index !== -1) {
            this.currentProductStocks[index].stock = newStockValue;
            this.currentProductStocks = [...this.currentProductStocks];
          }

          // Cập nhật totalStock trong products array
          const productIndex = this.products.findIndex(
            (p) => p.id === this.selectedProduct.id
          );
          if (productIndex !== -1) {
            const diff = newStockValue - oldValue;
            this.products[productIndex].totalStock += diff;
            this.selectedProduct.totalStock += diff;
          }

          this.cdr.detectChanges();
          this.toastr.success('Stock updated!');
        },
        error: (err) => {
          console.error('Update stock error:', err);
          this.toastr.error('Failed to update stock');
        },
      });
  }

  deleteStock(stockId: number) {
    if (!confirm('Delete this stock entry?')) return;

    console.log('Deleting stock:', stockId);

    // Lưu lại stock value trước khi xóa
    const stockToDelete = this.currentProductStocks.find(
      (s) => s.id === stockId
    );
    if (!stockToDelete) {
      console.error('Stock not found in current list');
      return;
    }

    const deletedStockValue = stockToDelete.stock;
    console.log('Stock value to delete:', deletedStockValue);

    this.productService
      .deleteProductStock(this.selectedProduct.id, stockId)
      .subscribe({
        next: (response) => {
          console.log('Delete stock response:', response);

          this.toastr.success('Stock deleted!');

          // Xóa khỏi UI
          this.currentProductStocks = this.currentProductStocks.filter(
            (s) => s.id !== stockId
          );

          // Cập nhật totalStock bằng cách tính lại từ currentProductStocks
          const newTotalStock = this.currentProductStocks.reduce(
            (sum, s) => sum + s.stock,
            0
          );
          console.log('New total stock after delete:', newTotalStock);

          const productIndex = this.products.findIndex(
            (p) => p.id === this.selectedProduct.id
          );
          if (productIndex !== -1) {
            this.products[productIndex].totalStock = newTotalStock;
            this.selectedProduct.totalStock = newTotalStock;
            console.log('Updated product totalStock to:', newTotalStock);
          }

          // Force change detection
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Delete stock error:', err);
          this.toastr.error('Failed to delete stock');
        },
      });
  }

  closeProductModal() {
    this.showProductModal = false;
    this.validationErrors = {
      name: '',
      imageUrl: '',
      price: '',
      description: '',
      categoryId: '',
    };
  }

  closeStockModal() {
    this.showStockModal = false;
  }

  // Helper method để cập nhật totalStock trong products array
  updateProductTotalStock(productId: number, stocks: any[]) {
    const totalStock = stocks.reduce((sum, stock) => sum + stock.stock, 0);
    const productIndex = this.products.findIndex((p) => p.id === productId);
    if (productIndex !== -1) {
      this.products[productIndex].totalStock = totalStock;
      this.selectedProduct.totalStock = totalStock;
    }
  }

  trackById(index: number, item: any) {
    return item.id;
  }

  validateProduct(product: any): boolean {
    this.validationErrors = {
      name: '',
      imageUrl: '',
      price: '',
      description: '',
      categoryId: '',
    };

    let isValid = true;

    if (!product.name || product.name.trim() === '') {
      this.validationErrors.name = 'Product name is required';
      isValid = false;
    } else if (product.name.trim().length < 3) {
      this.validationErrors.name = 'Product name must be at least 3 characters';
      isValid = false;
    } else if (product.name.trim().length > 100) {
      this.validationErrors.name =
        'Product name must not exceed 100 characters';
      isValid = false;
    }

    if (!product.imageUrl || product.imageUrl.trim() === '') {
      this.validationErrors.imageUrl = 'Main image URL is required';
      isValid = false;
    }

    if (
      product.price === null ||
      product.price === undefined ||
      product.price === ''
    ) {
      this.validationErrors.price = 'Price is required';
      isValid = false;
    } else if (product.price <= 0) {
      this.validationErrors.price = 'Price must be greater than 0';
      isValid = false;
    } else if (product.price > 999999) {
      this.validationErrors.price = 'Price is too high';
      isValid = false;
    }

    if (!product.description || product.description.trim() === '') {
      this.validationErrors.description = 'Description is required';
      isValid = false;
    } else if (product.description.trim().length < 10) {
      this.validationErrors.description =
        'Description must be at least 10 characters';
      isValid = false;
    } else if (product.description.trim().length > 1000) {
      this.validationErrors.description =
        'Description must not exceed 1000 characters';
      isValid = false;
    }

    if (!product.categoryId || product.categoryId === '') {
      this.validationErrors.categoryId = 'Please select a category';
      isValid = false;
    }

    return isValid;
  }
}
