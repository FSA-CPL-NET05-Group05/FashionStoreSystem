import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = 'https://localhost:7057/api/Accounts';

  constructor(private http: HttpClient) {}

  getUsers(page: number, pageSize: number): Observable<any> {
    // KHÔNG cần thêm token thủ công nữa - interceptor đã xử lý
    return this.http.get<any>(
      `${this.apiUrl}/paged?page=${page}&pageSize=${pageSize}`
    );
  }

  banUser(id: string, reason: string = 'Admin action'): Observable<any> {
    // KHÔNG cần thêm token thủ công nữa - interceptor đã xử lý
    return this.http.put(
      `${this.apiUrl}/${id}/lock`,
      { reason }
    );
  }

  unbanUser(id: string, reason: string = 'Admin action'): Observable<any> {
    // KHÔNG cần thêm token thủ công nữa - interceptor đã xử lý
    return this.http.put(
      `${this.apiUrl}/${id}/unlock`,
      { reason }
    );
  }
}