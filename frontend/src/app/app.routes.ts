import { Routes } from '@angular/router';

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
<<<<<<< HEAD
            path: 'gerente',
            loadChildren: () => import('./features/gerente/gerente.routes').then(m => m.GERENTE_ROUTES)
=======
            path: 'gerente', loadChildren: () =>
                import('./features/gerente/gerente.routes').then(m => m.GERENTE_ROUTES)
>>>>>>> 233c997ba5d0860b7ee3801cd44172166b3be219
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
