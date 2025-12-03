import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastr = inject(ToastrService);

  const token = localStorage.getItem('token');

  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  return next(authReq).pipe(
    catchError((error) => {
      const shouldSkip =
        req.url.includes('/login') ||
        req.url.includes('/Feedback') ||
        req.url.includes('/feedback');

      if (error.status === 401 && !shouldSkip) {
        localStorage.removeItem('token');
        localStorage.removeItem('currentUser');

        toastr.warning('Session expired. Please login again.');
        router.navigate(['/']);
      }

      return throwError(() => error);
    })
  );
};
