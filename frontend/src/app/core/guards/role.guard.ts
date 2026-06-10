import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if(!authService.esAutenticado()){
    return router.createUrlTree(['/login']);
  }


  const rolesRequeridos = route.data?.['roles'] as string[];

  if (!rolesRequeridos || authService.tieneRoles(rolesRequeridos)) {
    return true;
  }
  
  return router.createUrlTree([authService.obtenerRutaHome()]);
};
