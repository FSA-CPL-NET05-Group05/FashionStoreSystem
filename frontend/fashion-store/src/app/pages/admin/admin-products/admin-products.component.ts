import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { ToastrService } from 'ngx-toastr';
import { catchError, forkJoin, of } from 'rxjs';
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
  currentProductSizes: any[] = [];

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
    imagesText: '',
  };

  stockForm: any = {
    sizeId: '',
    colorId: '',
    stock: 0,
  };

  // Object lưu trữ lỗi validation
  validationErrors: any = {
    name: '',
    imageUrl: '',
    price: '',
    description: '',
    categoryId: ''
  };

  constructor(
    private productService: ProductService,
    private toastr: ToastrService,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    forkJoin({
      products: this.productService.getProducts(),
      categories: this.productService.getCategories(),
      sizes: this.productService.getSizes(),
      colors: this.productService.getColors(),
    }).subscribe({
      next: ({ products, categories, sizes, colors }) => {
        this.products = products;
        this.categories = categories;
        this.sizes = sizes;
        this.colors = colors;
      },
    });
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

  openAddModal() {
    this.isEditMode = false;
    this.productForm = {
      name: '',
      description: '',
      price: 0,
      categoryId: '',
      imageUrl: '',
      imagesText: '',
    };
    // Reset validation errors
    this.validationErrors = {
      name: '',
      imageUrl: '',
      price: '',
      description: '',
      categoryId: ''
    };
    this.showProductModal = true;
  }

  editProduct(product: any) {
    this.isEditMode = true;
    this.productForm = {
      ...product,
      imagesText: product.images?.join(', ') || '',
    };
    // Reset validation errors
    this.validationErrors = {
      name: '',
      imageUrl: '',
      price: '',
      description: '',
      categoryId: ''
    };
    this.showProductModal = true;
  }

  saveProduct(e: Event) {
    e.preventDefault();

    // Validate form trước khi submit
    if (!this.validateProduct(this.productForm)) {
      this.toastr.warning('Please fix all validation errors');
      return;
    }

    const data: any = {
      ...this.productForm,
      price: +this.productForm.price,
      categoryId: +this.productForm.categoryId,
      images: this.productForm.imagesText
        ? this.productForm.imagesText
            .split(',')
            .map((url: string) => url.trim())
        : [this.productForm.imageUrl],
    };
    delete data.imagesText;

    if (this.isEditMode) {
      this.productService.updateProduct(data.id, data).subscribe({
        next: () => {
          this.toastr.success('Product updated!');
          this.closeProductModal();
          this.loadData();
        },
        error: () => this.toastr.error('Failed to update product'),
      });
    } else {
      this.productService.createProduct(data).subscribe({
        next: () => {
          this.toastr.success('Product added!');
          this.closeProductModal();
          this.loadData();
        },
        error: () => this.toastr.error('Failed to add product'),
      });
    }
  }

  // deleteProduct(id: number) {
  //   if (
  //     !confirm(
  //       'Delete this product? This will remove all related data including orders, reviews, and cart items.'
  //     )
  //   )
  //     return;

  //   forkJoin({
  //     productSizes: this.productService.getProductSizes(id),
  //     cart: this.http.get<any[]>(`http://localhost:3000/cart?productId=${id}`),
  //     orderDetails: this.http.get<any[]>(`http://localhost:3000/orderDetails?productId=${id}`),
  //     feedbacks: this.http.get<any[]>(`http://localhost:3000/feedbacks?productId=${id}`),
  //     purchaseHistory: this.http.get<any[]>(`http://localhost:3000/purchaseHistory?productId=${id}`),
  //   }).subscribe({
  //     next: ({ productSizes, cart, orderDetails, feedbacks, purchaseHistory }) => {
  //       const deleteRequests: any[] = [];

  //       productSizes.forEach((ps) => {
  //         deleteRequests.push(
  //           this.productService.deleteProductSize(ps.id).pipe(
  //             catchError(() => of(null))
  //           )
  //         );
  //       });

  //       cart.forEach((item) => {
  //         deleteRequests.push(
  //           this.http.delete(`http://localhost:3000/cart/${item.id}`).pipe(
  //             catchError(() => of(null))
  //           )
  //         );
  //       });

  //       orderDetails.forEach((detail) => {
  //         deleteRequests.push(
  //           this.http.delete(`http://localhost:3000/orderDetails/${detail.id}`).pipe(
  //             catchError(() => of(null))
  //           )
  //         );
  //       });

  //       feedbacks.forEach((feedback) => {
  //         deleteRequests.push(
  //           this.http.delete(`http://localhost:3000/feedbacks/${feedback.id}`).pipe(
  //             catchError(() => of(null))
  //           )
  //         );
  //       });

  //       purchaseHistory.forEach((history) => {
  //         deleteRequests.push(
  //           this.http.delete(`http://localhost:3000/purchaseHistory/${history.id}`).pipe(
  //             catchError(() => of(null))
  //           )
  //         );
  //       });

  //       if (deleteRequests.length > 0) {
  //         forkJoin(deleteRequests).subscribe({
  //           next: () => {
  //             this.productService.deleteProduct(id).subscribe({
  //               next: () => {
  //                 this.toastr.success('Product and all related data deleted!');
  //                 this.loadData();
  //               },
  //             });
  //           },
  //         });
  //       } else {
  //         this.productService.deleteProduct(id).subscribe({
  //           next: () => {
  //             this.toastr.success('Product deleted!');
  //             this.loadData();
  //           },
  //         });
  //       }
  //     },
  //     error: () => this.toastr.error('Failed to load product data'),
  //   });
  // }
 deleteProduct(id: number) {
    if (
      !confirm(
        'Delete this product? This will remove all related data including orders, reviews, and cart items.'
      )
    )
      return;

    // Lấy tất cả dữ liệu liên quan
    forkJoin({
      productSizes: this.productService.getProductSizes(id),
      cart: this.http.get<any[]>(`http://localhost:3000/cart?productId=${id}`),
      orderDetails: this.http.get<any[]>(
        `http://localhost:3000/orderDetails?productId=${id}`
      ),
      feedbacks: this.http.get<any[]>(
        `http://localhost:3000/feedbacks?productId=${id}`
      ),
      purchaseHistory: this.http.get<any[]>(
        `http://localhost:3000/purchaseHistory?productId=${id}`
      ),
    }).subscribe({
      next: ({
        productSizes,
        cart,
        orderDetails,
        feedbacks,
        purchaseHistory,
      }) => {
        const deleteRequests: any[] = [];

        // Thêm các request xóa productSizes với error handling
        productSizes.forEach((ps) => {
          deleteRequests.push(
            this.productService.deleteProductSize(ps.id).pipe(
              catchError((err) => {
                console.warn(
                  `ProductSize ${ps.id} already deleted or not found`
                );
                return of(null); // Bỏ qua lỗi và tiếp tục
              })
            )
          );
        });

        // Thêm các request xóa cart items với error handling
        cart.forEach((item) => {
          deleteRequests.push(
            this.http.delete(`http://localhost:3000/cart/${item.id}`).pipe(
              catchError((err) => {
                console.warn(
                  `Cart item ${item.id} already deleted or not found`
                );
                return of(null);
              })
            )
          );
        });

        // Thêm các request xóa orderDetails với error handling
        orderDetails.forEach((detail) => {
          deleteRequests.push(
            this.http
              .delete(`http://localhost:3000/orderDetails/${detail.id}`)
              .pipe(
                catchError((err) => {
                  console.warn(
                    `OrderDetail ${detail.id} already deleted or not found`
                  );
                  return of(null);
                })
              )
          );
        });

        // Thêm các request xóa feedbacks với error handling
        feedbacks.forEach((feedback) => {
          deleteRequests.push(
            this.http
              .delete(`http://localhost:3000/feedbacks/${feedback.id}`)
              .pipe(
                catchError((err) => {
                  console.warn(
                    `Feedback ${feedback.id} already deleted or not found`
                  );
                  return of(null);
                })
              )
          );
        });

        // Thêm các request xóa purchaseHistory với error handling
        purchaseHistory.forEach((history) => {
          deleteRequests.push(
            this.http
              .delete(`http://localhost:3000/purchaseHistory/${history.id}`)
              .pipe(
                catchError((err) => {
                  console.warn(
                    `PurchaseHistory ${history.id} already deleted or not found`
                  );
                  return of(null);
                })
              )
          );
        });

        // Nếu có dữ liệu liên quan thì xóa tất cả trước
        if (deleteRequests.length > 0) {
          forkJoin(deleteRequests)
            .pipe(
              catchError((err) => {
                console.warn(
                  'Some items could not be deleted, but continuing...',
                  err
                );
                return of(null); // Bỏ qua lỗi tổng thể
              })
            )
            .subscribe({
              next: () => {
                // Sau khi xóa hết dữ liệu liên quan, xóa product
                this.productService
                  .deleteProduct(id)
                  .pipe(
                    catchError((err) => {
                      console.warn(
                        `Product ${id} already deleted or not found`
                      );
                      return of(null);
                    })
                  )
                  .subscribe({
                    next: () => {
                      this.toastr.success(
                        'Product and all related data deleted!'
                      );
                      this.loadData();
                    },
                  });
              },
            });
        } else {
          // Nếu không có dữ liệu liên quan thì xóa product luôn
          this.productService
            .deleteProduct(id)
            .pipe(
              catchError((err) => {
                console.warn(`Product ${id} already deleted or not found`);
                return of(null);
              })
            )
            .subscribe({
              next: () => {
                this.toastr.success('Product deleted!');
                this.loadData();
              },
            });
        }
      },
      error: (err) => {
        console.error('Load related data error:', err);
        this.toastr.error('Failed to load product data');
      },
    });
  }

  manageStock(product: any) {
    this.selectedProduct = product;
    this.productService.getProductSizes(product.id).subscribe({
      next: (sizes) => {
        this.currentProductSizes = sizes;
        this.showStockModal = true;
      },
      error: () => this.toastr.error('Failed to load stock data'),
    });
  }

  addStock() {
    if (!this.stockForm.sizeId || !this.stockForm.colorId || this.stockForm.stock < 0) {
      this.toastr.warning('Please fill all fields');
      return;
    }

    const newStock = {
      productId: this.selectedProduct.id,
      sizeId: +this.stockForm.sizeId,
      colorId: +this.stockForm.colorId,
      stock: +this.stockForm.stock,
    };

    this.productService.createProductSize(newStock).subscribe({
      next: () => {
        this.toastr.success('Stock added!');
        this.manageStock(this.selectedProduct);
        this.stockForm = { sizeId: '', colorId: '', stock: 0 };
      },
      error: () => this.toastr.error('Failed to add stock'),
    });
  }

  updateStock(ps: any, newStock: number) {
    if (newStock < 0) return;

    this.productService.updateProductSizeStock(ps.id, newStock).subscribe({
      next: () => {
        ps.stock = newStock;
        this.toastr.success('Stock updated!');
      },
      error: () => this.toastr.error('Failed to update stock'),
    });
  }

  deleteStock(id: number) {
    if (!confirm('Delete this stock entry?')) return;

    this.productService.deleteProductSize(id).subscribe({
      next: () => {
        this.toastr.success('Stock deleted!');
        this.manageStock(this.selectedProduct);
      },
      error: () => this.toastr.error('Failed to delete stock'),
    });
  }

  closeProductModal() {
    this.showProductModal = false;
    // Reset validation errors
    this.validationErrors = {
      name: '',
      imageUrl: '',
      price: '',
      description: '',
      categoryId: ''
    };
  }

  closeStockModal() {
    this.showStockModal = false;
  }

  trackById(index: number, item: any) {
    return item.id;
  }

  // ==================== VALIDATION ====================

  // Hàm validate product
  validateProduct(product: any): boolean {
    // Reset lỗi
    this.validationErrors = {
      name: '',
      imageUrl: '',
      price: '',
      description: '',
      categoryId: ''
    };

    let isValid = true;

    // Validate name
    if (!product.name || product.name.trim() === '') {
      this.validationErrors.name = 'Product name is required';
      isValid = false;
    } else if (product.name.trim().length < 3) {
      this.validationErrors.name = 'Product name must be at least 3 characters';
      isValid = false;
    } else if (product.name.trim().length > 100) {
      this.validationErrors.name = 'Product name must not exceed 100 characters';
      isValid = false;
    }

    

    // Validate price
    if (product.price === null || product.price === undefined || product.price === '') {
      this.validationErrors.price = 'Price is required';
      isValid = false;
    } else if (product.price <= 0) {
      this.validationErrors.price = 'Price must be greater than 0';
      isValid = false;
    } else if (product.price > 999999) {
      this.validationErrors.price = 'Price is too high';
      isValid = false;
    }

    // Validate description
    if (!product.description || product.description.trim() === '') {
      this.validationErrors.description = 'Description is required';
      isValid = false;
    } else if (product.description.trim().length < 10) {
      this.validationErrors.description = 'Description must be at least 10 characters';
      isValid = false;
    } else if (product.description.trim().length > 1000) {
      this.validationErrors.description = 'Description must not exceed 1000 characters';
      isValid = false;
    }

    // Validate categoryId
    if (!product.categoryId || product.categoryId === '') {
      this.validationErrors.categoryId = 'Please select a category';
      isValid = false;
    }

    return isValid;
  }

//test delete
testDelete(id: number) {
  console.log('Testing simple delete for ID:', id);
  
  this.productService.deleteProduct(id).subscribe({
    next: (response) => {
      console.log('✅ Delete successful:', response);
      this.toastr.success('Product deleted!');
      this.loadData();
    },
    error: (error) => {
      console.error('❌ Delete failed:', error);
      console.error('Status:', error.status);
      console.error('Message:', error.message);
      this.toastr.error(`Delete failed: ${error.message}`);
    }
  });
}



}