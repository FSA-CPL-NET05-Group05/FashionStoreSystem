import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr'; // Thêm Toastr
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastr = inject(ToastrService); // Khởi tạo Toastr

  // Lấy token từ localStorage (key là 'token')
  const token = localStorage.getItem('token');

  // Clone request và thêm Authorization header nếu có token
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });

    console.log('🔐 Request với token:', {
      url: req.url,
      method: req.method,
      hasToken: true,
    });
  } else {
    console.warn('⚠️ Request không có token:', req.url);
  }

  // Xử lý response và error
  return next(authReq).pipe(
    catchError((error) => {
      if (error.status === 401) {
        if (error.error?.message === 'User is banned') {
          // Hiển thị Toast khác cho người dùng bị ban
          toastr.error('Your account has been banned. Please contact support.');
        } else {
          console.error(
            '🚫 401 Unauthorized - Token không hợp lệ hoặc đã hết hạn'
          );

          // Xóa token và user data
          localStorage.removeItem('token');
          localStorage.removeItem('currentUser');

          // Redirect về trang home (có modal login)
          router.navigate(['/']);
        }
      }

      return throwError(() => error);
    })
  );
};
