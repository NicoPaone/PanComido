import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Functional Router Guard to check if current user has the required roles.
 * NOTE: Replace this mock guard with real backend JWT authentication check in the future.
 */
export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const requiredRoles = route.data?.['roles'] as string[];

  if (!requiredRoles || authService.hasRole(requiredRoles)) {
    return true;
  }

  // Redirect to sandbox page if not authorized
  router.navigate(['/staff/prueba']);
  return false;
};
