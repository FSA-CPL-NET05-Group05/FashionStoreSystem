import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.currentUserValue?.role === 'Admin') return true;

  router.navigate(['/']);
  return false;
};
