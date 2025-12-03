import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { Router } from '@angular/router';
import { User } from '../models/models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'https://localhost:7057/api/Auth';

  private currentUserSubject = new BehaviorSubject<User | null>(
    JSON.parse(localStorage.getItem('currentUser') || 'null')
  );
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    const token = localStorage.getItem('token');
    const user = this.currentUserSubject.value;
  }

  get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  login(username: string, password: string): Observable<User | null> {
    return this.http
      .post<User & { token: string }>(`${this.apiUrl}/login`, {
        username,
        password,
      })
      .pipe(
        map((res) => {
          if (!res?.token) {
            throw new Error('Invalid credentials');
          }

          localStorage.setItem('token', res.token);

          const user: User = {
            id: res.id,
            username: res.username,
            fullName: res.fullName,
            role: res.role,
          };

          localStorage.setItem('currentUser', JSON.stringify(user));
          this.currentUserSubject.next(user);

          return user;
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
    return this.currentUserValue?.role === 'Admin';
  }

  isCustomer(): boolean {
    return this.currentUserValue?.role === 'Customer';
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
