import { Routes } from '@angular/router';
import { roleGuard } from './core/guards/role.guard';

import { DEFAULT_ROUTE } from './app.constants';
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
            loadChildren: () =>
               import('./features/gerente/gerente.routes').then(m => m.GERENTE_ROUTES)
         },

         {
            path: 'cocina',
            canActivate: [roleGuard],
            data: { roles: ['Cocina'] },
            loadChildren: () => import('./features/cocina/cocina.routes').then(m => m.COCINA_ROUTES)
         },
         {
            path: '',
            redirectTo: 'cocina',
            pathMatch: 'full'
         }
      ]
   },

   /* MOZO - Layout separado sin sidebar */
   {
      path: 'staff/mozo',
      canActivate: [roleGuard],
      data: { roles: ['Mozo'] },
      loadComponent: () => import('./layouts/mozo-layout/mozo-layout').then(m => m.MozoLayout),
      children: [
         {
            path: '',
            loadChildren: () => import('./features/mozo/mozo.routes').then(m => m.MOZO_ROUTES)
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
