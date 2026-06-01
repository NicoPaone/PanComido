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

  // NOTE: El endpoint del back para verificar el token o rol de 
  // sesión debe integrarse aquí
 return router.createUrlTree([authService.getHomeRoute()]);
};
