import { Routes } from '@angular/router';

export const COMENSAL_ROUTES: Routes = [
  {

    path: 'escanear-mesa',
    loadComponent: () => import('./escanear-mesa/page/escanear-mesa').then(m => m.ScanQrComponent)
  },
  {
  path: 'nro-de-mesa',
  loadComponent: () =>
    import('./nro-de-mesa/page/nro-de-mesa')
      .then(m => m.NroDeMesaComponent)
},
{
  path: 'cantidad-personas',
  loadComponent: () =>
    import('./cantidad-personas/page/cantidad-personas')
      .then(m => m.CantidadPersonasComponent)
},
  {

    path: 'ver-carta',
    loadComponent: () => import('./ver-carta/page/ver-carta').then(m => m.VerCartaComponent)
  },

];

