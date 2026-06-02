import { Routes } from '@angular/router';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [

   {
      path: 'staff',
      loadComponent: () =>
         import('./layouts/staff-layout/staff-layout').then(m => m.StaffLayout),

      children: [

         {
            path: 'prueba',
            loadComponent: () =>
               import('./shared/components/prueba/prueba').then(m => m.Prueba)
         },

         {
            path: 'gerente',
            loadChildren: () =>
               import('./features/gerente/gerente.routes').then(m => m.GERENTE_ROUTES)
         },

         {
            path: '',
            redirectTo: 'prueba',
            pathMatch: 'full'
         }

      ]
   },

   /* COMENSAL */

   {
      path: 'comensal',

      children: [

         {
            path: 'ver-carta',
            loadComponent: () =>
               import('./features/comensal/ver-carta/page/ver-carta')
                  .then(m => m.VerCartaComponent)
         },

         {
            path: 'pedido',
            loadComponent: () =>
               import('./features/comensal/pedido/page/pedido')
                  .then(m => m.PedidoComponent)
         },

         {
            path: '',
            redirectTo: 'ver-carta',
            pathMatch: 'full'
         }

      ]
   },

];