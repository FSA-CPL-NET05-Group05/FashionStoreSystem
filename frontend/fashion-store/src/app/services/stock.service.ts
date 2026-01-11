import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, switchMap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StockService {
  private apiUrl = 'http://103.200.21.215:3000';

  constructor(private http: HttpClient) {}

  getStock(
    productId: number,
    sizeId: number,
    colorId: number
  ): Observable<any> {
    return this.http
      .get<any[]>(
        `${this.apiUrl}/productSizes?productId=${productId}&sizeId=${sizeId}&colorId=${colorId}`
      )
      .pipe(map((results) => results[0]));
  }

  checkStock(
    productId: number,
    sizeId: number,
    colorId: number,
    quantity: number
  ): Observable<boolean> {
    return this.getStock(productId, sizeId, colorId).pipe(
      map((productSize) => productSize && productSize.stock >= quantity)
    );
  }

  decreaseStock(
    productId: number,
    sizeId: number,
    colorId: number,
    quantity: number
  ): Observable<any> {
    return this.getStock(productId, sizeId, colorId).pipe(
      switchMap((productSize) => {
        if (!productSize) throw new Error('Product size not found');
        if (productSize.stock < quantity) throw new Error('Not enough stock');

        const newStock = productSize.stock - quantity;
        return this.http.patch(
          `${this.apiUrl}/productSizes/${productSize.id}`,
          { stock: newStock }
        );
      })
    );
  }

  increaseStock(
    productId: number,
    sizeId: number,
    colorId: number,
    quantity: number
  ): Observable<any> {
    return this.getStock(productId, sizeId, colorId).pipe(
      switchMap((productSize) => {
        if (!productSize) throw new Error('Product size not found');

        const newStock = productSize.stock + quantity;
        return this.http.patch(
          `${this.apiUrl}/productSizes/${productSize.id}`,
          { stock: newStock }
        );
      })
    );
  }
}
