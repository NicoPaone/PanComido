import { Routes } from '@angular/router';
import { roleGuard } from './core/guards/role.guard';

const DEFAULT_ROUTE = 'staff/gerente';

export const routes: Routes = [

   {
      path: 'staff',
      loadComponent: () =>
          import('./layouts/staff-layout/staff-layout').then(m => m.StaffLayout),

      children: [
          {
            path: 'gerente',
            canActivate: [roleGuard],
            data: { roles: ['Gerente'] },
            loadChildren: () => import('./features/gerente/gerente.routes').then(m => m.GERENTE_ROUTES)
         },
         {
            path: 'cocina',
            canActivate: [roleGuard],
            data: { roles: ['Cocina'] },
            loadChildren: () => import('./features/cocina/cocina.routes').then(m => m.COCINA_ROUTES)
         },

         {
            path: 'mozo',
            canActivate: [roleGuard],
            data: { roles: ['Mozo'] },
            loadChildren: () => import('./features/mozo/mozo.routes').then(m => m.MOZO_ROUTES)
         },
         {
            path: '',redirectTo: 'cocina', pathMatch: 'full'
         }

      ]
   },

   /* COMENSAL */

  {
  path: 'comensal',
 loadChildren: () => import('./features/comensal/comensal.routes').then(m => m.COMENSAL_ROUTES)
    
},
   {
      path: '', redirectTo: DEFAULT_ROUTE, pathMatch: 'full'
   },

   {
      path: '**', redirectTo: DEFAULT_ROUTE
   }
];