import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = 'http://localhost:3000';

  constructor(private http: HttpClient) {}

  getProducts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/products`);
  }

  getProduct(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/products/${id}`);
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
    return this.http.get<any[]>(`${this.apiUrl}/categories`);
  }

  updateProductSizeStock(id: number, stock: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/productSizes/${id}`, { stock });
  }
}
