import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, catchError, map } from 'rxjs';
import { Router } from '@angular/router';

export interface User {
  id: number;
  username: string;
  fullName: string;
  role: 'admin' | 'customer';
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'https://localhost:7057/api/Auth';

  private currentUserSubject = new BehaviorSubject<User | null>(
    JSON.parse(localStorage.getItem('currentUser') || 'null')
  );
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  login(username: string, password: string): Observable<User | null> {
    return this.http
      .post<{ username: string; token: string }>(`${this.apiUrl}/login`, {
        username,
        password,
      })
      .pipe(
        map((res) => {
          if (!res?.token) throw new Error('Invalid credentials');

          // Lưu token
          localStorage.setItem('token', res.token);

          // Tạo user giả dựa trên username
          const user: User = {
            id: 1, // tạm thời dùng 1, có thể random hoặc dùng DB nếu có API profile
            username: res.username,
            fullName: res.username,
            role: res.username.toLowerCase().includes('admin')
              ? 'admin'
              : 'customer',
          };

          localStorage.setItem('currentUser', JSON.stringify(user));
          this.currentUserSubject.next(user);

          return user;
        }),
        catchError((err) => {
          console.error(err);
          return of(null);
        })
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.router.navigate(['/']);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  isAdmin(): boolean {
    return this.currentUserValue?.role === 'admin';
  }

  isCustomer(): boolean {
    return this.currentUserValue?.role === 'customer';
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
