import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = 'http://localhost:3000';

  constructor(private http: HttpClient) {}

  createOrder(
    cartItems: any[],
    guestInfo?: any,
    userId?: string
  ): Observable<any> {
    const totalAmount = cartItems.reduce(
      (sum, item) => sum + (item.product?.price || 0) * item.quantity,
      0
    );

    const order: any = {
      orderDate: new Date().toISOString(),
      userId: userId || null,
      totalAmount: totalAmount,
      status: 'pending',
      ...(guestInfo && {
        guestName: guestInfo.name,
        guestEmail: guestInfo.email,
        guestPhone: guestInfo.phone,
      }),
    };

    return this.http.post<any>(`${this.apiUrl}/orders`, order).pipe(
      switchMap((createdOrder) => {
        // Tạo order details
        const orderDetails = cartItems.map((item) => ({
          orderId: createdOrder.id,
          productId: item.productId,
          quantity: item.quantity,
          price: item.product?.price || 0,
          sizeId: item.sizeId,
          colorId: item.colorId,
        }));

        const detailRequests = orderDetails.map((detail) =>
          this.http.post<any>(`${this.apiUrl}/orderDetails`, detail)
        );

        // Tạo purchase history nếu là customer
        let purchaseRequests: Observable<any>[] = [];
        if (userId) {
          purchaseRequests = cartItems.map((item) =>
            this.http.post(`${this.apiUrl}/purchaseHistory`, {
              userId: userId,
              productId: item.productId,
              orderId: createdOrder.id,
            })
          );
        }

        return forkJoin([...detailRequests, ...purchaseRequests]).pipe(
          map(() => createdOrder)
        );
      })
    );
  }

  getOrders(): Observable<any[]> {
    return forkJoin({
      orders: this.http.get<any[]>(`${this.apiUrl}/orders`),
      users: this.http.get<any[]>(`${this.apiUrl}/users`),
    }).pipe(
      map(({ orders, users }) => {
        return orders
          .map((order) => ({
            ...order,
            user: users.find((u) => u.id === order.userId),
          }))
          .sort(
            (a, b) =>
              new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime()
          );
      })
    );
  }

  getOrderDetails(orderId: number): Observable<any[]> {
    return forkJoin({
      details: this.http.get<any[]>(
        `${this.apiUrl}/orderDetails?orderId=${orderId}`
      ),
      products: this.http.get<any[]>(`${this.apiUrl}/products`),
      sizes: this.http.get<any[]>(`${this.apiUrl}/sizes`),
      colors: this.http.get<any[]>(`${this.apiUrl}/colors`),
    }).pipe(
      map(({ details, products, sizes, colors }) => {
        return details.map((detail) => ({
          ...detail,
          product: products.find((p) => p.id === detail.productId),
          size: sizes.find((s) => s.id === detail.sizeId),
          color: colors.find((c) => c.id === detail.colorId),
        }));
      })
    );
  }

  updateOrderStatus(id: number, status: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/orders/${id}`, { status });
  }
}
