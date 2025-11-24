import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  BehaviorSubject,
  catchError,
  forkJoin,
  map,
  Observable,
  of,
  switchMap,
  tap,
  throwError,
} from 'rxjs';
import { StockService } from './stock.service';

@Injectable({ providedIn: 'root' })
export class CartService {
  private apiUrl = 'http://localhost:3000';
  private cartCountSubject = new BehaviorSubject<number>(0);
  public cartCount$ = this.cartCountSubject.asObservable();

  constructor(private http: HttpClient, private stockService: StockService) {}

  private ensureLoggedIn(userId: string): boolean {
    return !!userId && userId !== 'guest';
  }

  getCart(userId: string): Observable<any[]> {
    if (!this.ensureLoggedIn(userId)) return of([]);

    return this.http.get<any[]>(`${this.apiUrl}/cart?userId=${userId}`);
  }

  getCartWithDetails(userId: string): Observable<any[]> {
    if (!this.ensureLoggedIn(userId)) return of([]);

    return forkJoin({
      cart: this.getCart(userId),
      products: this.http.get<any[]>(`${this.apiUrl}/products`),
      sizes: this.http.get<any[]>(`${this.apiUrl}/sizes`),
      colors: this.http.get<any[]>(`${this.apiUrl}/colors`),
    }).pipe(
      map(({ cart, products, sizes, colors }) =>
        cart.map((item) => ({
          ...item,
          product: products.find((p) => p.id === item.productId),
          size: sizes.find((s) => s.id === item.sizeId),
          color: colors.find((c) => c.id === item.colorId),
        }))
      )
    );
  }

  addToCart(item: any): Observable<any> {
    const { userId, productId, sizeId, colorId, quantity } = item;

    if (!this.ensureLoggedIn(userId)) {
      return throwError(() => new Error('You must login to add item to cart.'));
    }

    return this.stockService
      .checkStock(productId, sizeId, colorId, quantity)
      .pipe(
        switchMap((hasStock) => {
          if (!hasStock) {
            return throwError(() => new Error('Not enough stock available'));
          }

          return this.http.get<any[]>(
            `${this.apiUrl}/cart?userId=${userId}&productId=${productId}&sizeId=${sizeId}&colorId=${colorId}`
          );
        }),
        switchMap((existingItems) => {
          if (existingItems.length > 0) {
            const existingItem = existingItems[0];
            const newQuantity = existingItem.quantity + item.quantity;

            return this.stockService
              .decreaseStock(productId, sizeId, colorId, quantity)
              .pipe(
                switchMap(() =>
                  this.http.patch(`${this.apiUrl}/cart/${existingItem.id}`, {
                    quantity: newQuantity,
                    addedAt: new Date().toISOString(),
                  })
                )
              );
          } else {
            const newItem = {
              ...item,
              addedAt: new Date().toISOString(),
            };

            return this.stockService
              .decreaseStock(productId, sizeId, colorId, quantity)
              .pipe(
                switchMap(() => this.http.post(`${this.apiUrl}/cart`, newItem))
              );
          }
        }),
        tap(() => this.updateCartCount(userId))
      );
  }

  updateCartItem(
    cartItemId: number,
    newQuantity: number,
    oldQuantity: number,
    item: any
  ): Observable<any> {
    const diff = newQuantity - oldQuantity;

    if (diff > 0) {
      return this.stockService
        .decreaseStock(item.productId, item.sizeId, item.colorId, diff)
        .pipe(
          switchMap(() =>
            this.http.patch(`${this.apiUrl}/cart/${cartItemId}`, {
              quantity: newQuantity,
            })
          ),
          tap(() => this.updateCartCount(item.userId))
        );
    } else if (diff < 0) {
      return this.stockService
        .increaseStock(item.productId, item.sizeId, item.colorId, -diff)
        .pipe(
          switchMap(() =>
            this.http.patch(`${this.apiUrl}/cart/${cartItemId}`, {
              quantity: newQuantity,
            })
          ),
          tap(() => this.updateCartCount(item.userId))
        );
    }

    return of(null);
  }

  removeFromCart(cartItem: any): Observable<any> {
    return this.stockService
      .increaseStock(
        cartItem.productId,
        cartItem.sizeId,
        cartItem.colorId,
        cartItem.quantity
      )
      .pipe(
        switchMap(() => this.http.delete(`${this.apiUrl}/cart/${cartItem.id}`)),
        tap(() => this.updateCartCount(cartItem.userId)),
        catchError((error) => {
          console.error('Error in removeFromCart:', error);
          this.updateCartCount(cartItem.userId);
          return of({ success: true });
        })
      );
  }

  clearCartAfterOrder(userId: string): Observable<any> {
    if (!this.ensureLoggedIn(userId)) return of(null);

    return this.getCart(userId).pipe(
      switchMap((items) => {
        if (items.length === 0) return of(null);
        return forkJoin(
          items.map((item) =>
            this.http.delete(`${this.apiUrl}/cart/${item.id}`)
          )
        );
      }),
      tap(() => this.updateCartCount(userId))
    );
  }

  private updateCartCount(userId: string): void {
    if (!this.ensureLoggedIn(userId)) {
      this.cartCountSubject.next(0);
      return;
    }

    this.getCart(userId).subscribe({
      next: (items) => {
        this.cartCountSubject.next(items.length);
      },
      error: (err) => {
        console.error('Error updating cart count:', err);
        this.cartCountSubject.next(0);
      },
    });
  }

  getCurrentCartCount(): number {
    return this.cartCountSubject.value;
  }

  initializeCart(userId: string): void {
    this.updateCartCount(userId);
  }
}
