import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Blocks direct navigation to /tasks/new and /tasks/:id/edit for Employees -
// the UI already hides those links, this stops typing the URL directly.
export const manageTasksGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.canManageTasks() ? true : router.parseUrl('/tasks');
};
