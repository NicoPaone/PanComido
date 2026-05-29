import { Routes } from '@angular/router';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
   {
      path: 'staff',
      loadComponent: () => 
          import('./layouts/staff-layout/staff-layout').then(m => m.StaffLayout),
      children: [
         {
            path: 'prueba', loadComponent: () =>
                import('./shared/components/prueba/prueba').then(m => m.Prueba)
         },
         {
            path: 'gerente',
            canActivate: [roleGuard],
            data: { roles: ['Gerente'] },
            loadChildren: () => import('./features/gerente/gerente.routes').then(m => m.GERENTE_ROUTES)
         },
         {
            path: '',redirectTo: 'prueba', pathMatch: 'full'
         }
      ]
   },
   {
      path: '', redirectTo: 'staff/prueba', pathMatch: 'full'
   },

   {
      path: '**', redirectTo: 'staff/prueba'
   }
];
