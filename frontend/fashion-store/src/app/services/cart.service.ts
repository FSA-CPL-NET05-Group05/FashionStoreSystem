import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CartService {
  private apiUrl = 'http://localhost:5000/api/Cart';
  private cartCountSubject = new BehaviorSubject<number>(0);
  public cartCount$ = this.cartCountSubject.asObservable();

  constructor(private http: HttpClient) {}

  private getAuthHeaders() {
    const token = localStorage.getItem('token') || '';
    return { headers: new HttpHeaders({ Authorization: `Bearer ${token}` }) };
  }

  getCart(): Observable<any[]> {
    return this.http
      .get<any[]>(`${this.apiUrl}/get-my-cart`, this.getAuthHeaders())
      .pipe(tap((items) => this.cartCountSubject.next(items.length)));
  }

  addToCart(item: {
    productId: number;
    sizeId: number;
    colorId: number;
    quantity: number;
  }): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/add-to-cart`, item, this.getAuthHeaders())
      .pipe(tap(() => this.updateCartCount()));
  }

  updateCartItem(cartItemId: number, newQuantity: number): Observable<any> {
    return this.http
      .put(
        `${this.apiUrl}/update-quantity`,
        { cartItemId, newQuantity },
        this.getAuthHeaders()
      )
      .pipe(tap(() => this.updateCartCount()));
  }

  removeFromCart(cartItemId: number): Observable<any> {
    return this.http
      .delete(`${this.apiUrl}/remove/${cartItemId}`, this.getAuthHeaders())
      .pipe(tap(() => this.updateCartCount()));
  }

  private updateCartCount(): void {
    this.getCart().subscribe({
      next: (items) => this.cartCountSubject.next(items.length),
      error: () => this.cartCountSubject.next(0),
    });
  }

  clearCartCount(): void {
    this.cartCountSubject.next(0);
  }

  getCurrentCartCount(): number {
    return this.cartCountSubject.value;
  }
}
