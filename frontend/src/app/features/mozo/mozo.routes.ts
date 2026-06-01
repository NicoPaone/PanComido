import { Routes } from '@angular/router';

export const MOZO_ROUTES: Routes = [
  {
    path: 'mis-mesas',
    loadComponent: () => import('./pages/mis-mesas-page/mis-mesas-page').then(m => m.MisMesasPage)
  },
  {
    path: '',
    redirectTo: 'mis-mesas',
    pathMatch: 'full'
  }
];