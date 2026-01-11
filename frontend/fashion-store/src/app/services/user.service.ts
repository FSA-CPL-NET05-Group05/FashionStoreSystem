import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = 'http://localhost:5000/api/Accounts';

  constructor(private http: HttpClient) {}

  getUsers(page: number, pageSize: number): Observable<any> {
    const token = localStorage.getItem('token');
    return this.http.get<any>(
      `${this.apiUrl}/paged?page=${page}&pageSize=${pageSize}`,
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );
  }

  banUser(id: string, reason: string = 'Admin action'): Observable<any> {
    const token = localStorage.getItem('token');
    return this.http.put(
      `${this.apiUrl}/${id}/lock`,
      { reason },
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );
  }

  unbanUser(id: string, reason: string = 'Admin action'): Observable<any> {
    const token = localStorage.getItem('token');
    return this.http.put(
      `${this.apiUrl}/${id}/unlock`,
      { reason },
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );
  }
}
