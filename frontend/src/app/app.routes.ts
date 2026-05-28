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
            path: 'escanear-mesa',
            loadComponent: () =>
               import('./features/comensal/escanear-mesa/page/escanear-mesa')
                  .then(m => m.ScanQrComponent)
         },
         {
            path: 'nro-de-mesa',
            loadComponent: () =>
               import('./features/comensal/nro-de-mesa/page/nro-de-mesa')
                  .then(m => m.NroDeMesaComponent)
         },
         {
            path: 'cantidad-personas',
            loadComponent: () =>
               import('./features/comensal/cantidad-personas/page/cantidad-personas')
                  .then(m => m.CantidadPersonasComponent)
         },
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