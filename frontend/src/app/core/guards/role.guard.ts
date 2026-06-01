import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const rolesRequeridos = route.data?.['roles'] as string[];

  if (!rolesRequeridos || authService.hasRole(rolesRequeridos)) {
    return true;
  }

  // Usa la ruta del rol actual en vez de un string hardcodeado
  return router.createUrlTree([authService.getHomeRoute()]);
};