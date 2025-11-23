import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin, map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FeedbackService {
  private apiUrl = 'http://localhost:3000';

  constructor(private http: HttpClient) {}

  getFeedbacks(productId?: number): Observable<any[]> {
    const url = productId
      ? `${this.apiUrl}/feedbacks?productId=${productId}`
      : `${this.apiUrl}/feedbacks`;

    return forkJoin({
      feedbacks: this.http.get<any[]>(url),
      users: this.http.get<any[]>(`${this.apiUrl}/users`),
      products: this.http.get<any[]>(`${this.apiUrl}/products`),
    }).pipe(
      map(({ feedbacks, users, products }) => {
        return feedbacks
          .map((feedback) => ({
            ...feedback,
            user: users.find((u) => u.id === feedback.userId),
            product: products.find((p) => p.id === feedback.productId),
          }))
          .sort(
            (a, b) =>
              new Date(b.createdDate).getTime() -
              new Date(a.createdDate).getTime()
          );
      })
    );
  }

  createFeedback(feedback: any): Observable<any> {
    const newFeedback = {
      ...feedback,
      createdDate: new Date().toISOString(),
    };
    return this.http.post(`${this.apiUrl}/feedbacks`, newFeedback);
  }
}
