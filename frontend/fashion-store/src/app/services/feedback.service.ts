import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FeedbackService {
  private apiUrl = 'http://localhost:5000/api/Feedbacks';

  constructor(private http: HttpClient) {}

  getFeedbacks(productId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/product/${productId}`);
  }

  getAllFeedbacks(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  createFeedback(feedback: any): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) throw new Error('Token missing');

    const headers = { Authorization: `Bearer ${token}` };
    const newFeedback = { ...feedback, createdDate: new Date().toISOString() };

    return this.http.post(this.apiUrl, newFeedback, { headers });
  }
}
