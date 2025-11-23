import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StockService {
  private apiUrl = 'http://localhost:3000';

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
}
