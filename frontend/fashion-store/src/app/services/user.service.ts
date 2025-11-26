import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = 'http://localhost:3000/users';

  constructor(private http: HttpClient) {}

  getUsers(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  banUser(id: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}`, { status: 'banned' });
  }

  unbanUser(id: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}`, { status: 'active' });
  }
}
