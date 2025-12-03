import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'https://localhost:7057/api';

  constructor(private http: HttpClient) {}

  // ===== ADMIN PRODUCTS =====
  getAdminProducts(page: number = 1, pageSize: number = 10): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/AdminProducts`, {
      params: { page: page.toString(), pageSize: pageSize.toString() },
    });
  }

  getAdminProduct(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/AdminProducts/${id}`);
  }

  createProduct(product: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/AdminProducts`, product);
  }

  updateProduct(id: number, product: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/AdminProducts/${id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/AdminProducts/${id}`);
  }

  // ===== CUSTOMER PRODUCTS (for shop display) =====
  getProducts(): Observable<any[]> {
    // Lấy TẤT CẢ products với pageSize lớn
    return this.http
      .get<any>(`${this.apiUrl}/CustomerProduct`, {
        params: { page: '1', pageSize: '1000' },
      })
      .pipe(map((response) => response.items ?? []));
  }

  getProduct(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/CustomerProduct/${id}`);
  }

  getCategories(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Categories`);
  }

  getProductsByCategory(categoryId: number): Observable<any[]> {
    return this.http
      .get<any>(`${this.apiUrl}/CustomerProduct`, {
        params: {
          categoryId: categoryId.toString(),
          page: '1',
          pageSize: '1000', // Lấy tất cả trong category
        },
      })
      .pipe(map((res) => res.items ?? []));
  }

  // ===== STOCK MANAGEMENT =====
  getProductStocks(productId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/products/${productId}/stocks`);
  }

  getProductStock(productId: number, stockId: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/products/${productId}/stocks/${stockId}`
    );
  }

  createProductStock(productId: number, stock: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/products/${productId}/stocks`,
      stock
    );
  }

  updateProductStock(
    productId: number,
    stockId: number,
    stockValue: number
  ): Observable<any> {
    return this.http.put<any>(
      `${this.apiUrl}/products/${productId}/stocks/${stockId}`,
      { stock: stockValue }
    );
  }

  deleteProductStock(productId: number, stockId: number): Observable<any> {
    return this.http.delete<any>(
      `${this.apiUrl}/products/${productId}/stocks/${stockId}`
    );
  }
}
