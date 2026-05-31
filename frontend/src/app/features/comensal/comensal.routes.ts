import { Routes } from '@angular/router';

export const COMENSAL_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'ver-carta',
    pathMatch: 'full'
  },

  {
    path: 'escanear-mesa',
    loadComponent: () =>
      import('./escanear-mesa/page/escanear-mesa').then(m => m.ScanQr)
  },
  {
    path: 'nro-de-mesa',
    loadComponent: () =>
      import('./nro-de-mesa/page/nro-de-mesa').then(m => m.NroDeMesa)
  },
  {
    path: 'cantidad-personas',
    loadComponent: () =>
      import('./cantidad-personas/page/cantidad-personas').then(m => m.CantidadPersonas)
  },
  {
    path: 'ver-carta',
    loadComponent: () =>
      import('./ver-carta/page/ver-carta').then(m => m.VerCarta)
  },
  {
    path: 'pedido',
    loadComponent: () =>
      import('./pedido/page/pedido')
        .then(m => m.Pedido)
  },
  {
    path: 'personalizar-plato',
    loadComponent: () =>
      import('./personalizar-plato/page/personalizar-plato')
        .then(m => m.PersonalizarPlato)
  }
];

