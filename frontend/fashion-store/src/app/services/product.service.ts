import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Category, Product } from '../models/models';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'http://localhost:3000';
  http = inject(HttpClient);

  // GET
  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/products`);
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/products/${id}`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
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
}
