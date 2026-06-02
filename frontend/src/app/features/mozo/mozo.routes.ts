import { Routes } from '@angular/router';

export const MOZO_ROUTES: Routes = [
  {
    path: 'mis-mesas',
    loadComponent: () => import('./pages/mis-mesas/mis-mesas').then(m => m.MisMesasPage)
  },
  {

    path: 'llamados',
    loadComponent: () => import('./pages/llamados-mozo-page/llamados-mozo-page').then(m => m.LlamadosMozoPage)
  },
  {
    path: '',
    redirectTo: 'mis-mesas',
    pathMatch: 'full'
  }
];