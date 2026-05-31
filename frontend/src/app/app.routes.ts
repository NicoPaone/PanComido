import { Routes } from '@angular/router';
import { roleGuard } from './core/guards/role.guard';
//import { DEFAULT_ROUTE } from './app.config';

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
            path: '',redirectTo: 'cocina', pathMatch: 'full'
         }
      ]
   },
   {
      path: '', redirectTo: DEFAULT_ROUTE, pathMatch: 'full'
   },

   {
      path: '**', redirectTo: DEFAULT_ROUTE
   }
];
