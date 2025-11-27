import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = 'https://localhost:7057/api';

  constructor(private http: HttpClient) {}

  private getAuthHeaders() {
    const token = localStorage.getItem('token') || '';
    return { headers: new HttpHeaders({ Authorization: `Bearer ${token}` }) };
  }

  /** Tạo đơn hàng */
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

  /** Lấy danh sách đơn hàng với phân trang */
  getOrders(page: number = 1, pageSize: number = 8): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/Order?page=${page}&pageSize=${pageSize}`,
      this.getAuthHeaders()
    );
  }

  /** Lấy chi tiết đơn hàng */
  getOrderDetails(orderId: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/Order/${orderId}`,
      this.getAuthHeaders()
    );
  }

  /** Cập nhật trạng thái đơn hàng */
  updateOrderStatus(orderId: number, status: string): Observable<any> {
    return this.http.patch(
      `${this.apiUrl}/Order/update-status/${orderId}`,
      { status },
      this.getAuthHeaders()
    );
  }
}
