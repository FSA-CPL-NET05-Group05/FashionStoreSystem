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
    console.log(' Request với token:', {
      url: req.url,
      method: req.method,
      hasToken: true,
    });
  } else {
    console.warn(' Request không có token:', req.url);
  }
  
 
  return next(authReq).pipe(
    catchError((error) => {
      console.error(' HTTP Error:', {
        status: error.status,
        url: req.url,
        message: error.error?.message || error.message
      });

      
      const urlLower = req.url.toLowerCase();
      const isAuthEndpoint = urlLower.includes('/auth/login') || 
                             urlLower.includes('/auth/register') ||
                             urlLower.includes('login') ||
                             urlLower.includes('register');
      
      if (isAuthEndpoint) {
        console.log('⏭️ Bỏ qua xử lý lỗi cho endpoint auth:', req.url);
        
        return throwError(() => error);
      }
      
      if (error.status === 401) {
      
        if (error.error?.message?.toLowerCase().includes('ban')) {
          toastr.error('Tài khoản của bạn đã bị khóa. Vui lòng liên hệ hỗ trợ.', 'Tài khoản bị khóa');
          localStorage.removeItem('token');
          localStorage.removeItem('currentUser');
          router.navigate(['/']);
        } 
     
        else {
          console.error('🚫 401 Unauthorized - Token không hợp lệ hoặc đã hết hạn');
          toastr.warning('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.', 'Hết phiên');
          localStorage.removeItem('token');
          localStorage.removeItem('currentUser');
          router.navigate(['/']);
        }
      }
     
      else if (error.status === 403) {
        if (error.error?.message?.toLowerCase().includes('ban')) {
          toastr.error('Tài khoản của bạn đã bị khóa.', 'Tài khoản bị khóa');
          localStorage.removeItem('token');
          localStorage.removeItem('currentUser');
          router.navigate(['/']);
        } else {
          toastr.error('Bạn không có quyền truy cập.', 'Từ chối truy cập');
        }
      }
      
      else if (error.status === 500) {
        toastr.error('Lỗi server. Vui lòng thử lại sau.', 'Lỗi hệ thống');
      }
      
      else if (error.status === 0) {
        toastr.error('Không thể kết nối đến server.', 'Lỗi kết nối');
      }
      
      return throwError(() => error);
    })
  );
};