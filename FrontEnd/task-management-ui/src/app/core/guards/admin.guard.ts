import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Blocks the Audit Logs route for non-Admins, even by direct URL - the API
// enforces the same rule with [Authorize(Roles = "Admin")].
export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isAdmin() ? true : router.parseUrl('/dashboard');
};
