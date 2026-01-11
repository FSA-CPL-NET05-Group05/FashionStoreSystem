import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = 'http://103.200.21.215:5000/api';

  constructor(private http: HttpClient) {}

  private getAuthHeaders() {
    const token = localStorage.getItem('token') || '';
    return { headers: new HttpHeaders({ Authorization: `Bearer ${token}` }) };
  }

  createOrder(
    cartItems: any[],
    guestInfo?: any,
    userId?: string
  ): Observable<any> {
    const itemsPayload = cartItems.map((item) => ({
      productId: item.productId,
      sizeId: item.sizeId,
      colorId: item.colorId,
      quantity: item.quantity,
    }));

    const payload: any = {
      customerName: guestInfo?.name || '',
      customerEmail: guestInfo?.email || '',
      customerPhone: guestInfo?.phone || '',
      items: itemsPayload,
      userId: userId || null,
    };

    return this.http.post(
      `${this.apiUrl}/Order/place-order`,
      payload,
      this.getAuthHeaders()
    );
  }

  getOrders(page: number = 1, pageSize: number = 8): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/Order?page=${page}&pageSize=${pageSize}`,
      this.getAuthHeaders()
    );
  }

  getOrderDetails(orderId: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/Order/${orderId}`,
      this.getAuthHeaders()
    );
  }

  updateOrderStatus(orderId: number, status: string): Observable<any> {
    return this.http.patch(
      `${this.apiUrl}/Order/update-status/${orderId}`,
      { status },
      this.getAuthHeaders()
    );
  }
}
