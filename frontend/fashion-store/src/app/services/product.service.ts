import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'https://localhost:7057/api';

  constructor(private http: HttpClient) {}

  // ===== READ =====
  getProducts(): Observable<any[]> {
    return this.http
      .get<any>(`${this.apiUrl}/CustomerProduct`)
      .pipe(map((response) => response.items ?? []));
  }
  getProduct(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/CustomerProduct/${id}`);
  }

  getProductsWithDetails(): Observable<any[]> {
    return forkJoin({
      products: this.getProducts(),
      categories: this.getCategories(),
    }).pipe(
      map(({ products, categories }) => {
        return products.map((product) => ({
          ...product,
          category: categories.find((c) => c.id === product.categoryId),
        }));
      })
    );
  }

  getProductSizes(productId: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/productSizes?productId=${productId}`
    );
  }

  getSizes(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/sizes`);
  }

  getColors(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/colors`);
  }

  getCategories(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Categories`);
  }

  getProductsByCategory(categoryId: number): Observable<any[]> {
    return this.http
      .get<any>(`${this.apiUrl}/CustomerProduct`, {
        params: { categoryId: categoryId.toString() },
      })
      .pipe(map((res) => res.items ?? []));
  }

  // ===== CREATE =====
  createProduct(product: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/products`, product);
  }

  createProductSize(productSize: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/productSizes`, productSize);
  }

  // ===== UPDATE =====
  updateProduct(id: number, product: any): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/products/${id}`, product);
  }

  updateProductSizeStock(id: number, stock: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/productSizes/${id}`, { stock });
  }

  updateProductSize(id: number, data: any): Observable<any> {
    return this.http.patch(`${this.apiUrl}/productSizes/${id}`, data);
  }

  // ===== DELETE =====
  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/products/${id}`);
  }

  deleteProductSize(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/productSizes/${id}`);
  }
}
