import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BehaviorSubject,
  Observable,
  catchError,
  map,
  of,
  switchMap,
} from 'rxjs';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:3000';

  private currentUserSubject = new BehaviorSubject<any>(
    JSON.parse(localStorage.getItem('currentUser') || 'null')
  );
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  get currentUserValue() {
    return this.currentUserSubject.value;
  }

  login(username: string, password: string): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/auth/login`, { username, password })
      .pipe(
        catchError(() => of(null)),

        switchMap(() =>
          this.http.get<any[]>(
            `${this.apiUrl}/users?username=${username}&password=${password}`
          )
        ),

        map((users) => {
          if (!users.length) throw new Error('Invalid credentials');

          const user = users[0];

          if (user.status === 'banned') {
            throw new Error('Your account has been banned');
          }

          const fakeToken = 'FAKE_JWT_' + user.id;

          localStorage.setItem('token', fakeToken);
          localStorage.setItem('currentUser', JSON.stringify(user));

          this.currentUserSubject.next(user);
          return { user, token: fakeToken };
        })
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.router.navigate(['/']);
  }

  isLoggedIn() {
    return !!localStorage.getItem('token');
  }

  isAdmin() {
    return this.currentUserValue?.role === 'admin';
  }

  isCustomer() {
    return this.currentUserValue?.role === 'customer';
  }
}
