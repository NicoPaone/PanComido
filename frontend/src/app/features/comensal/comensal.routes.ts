import { Routes } from '@angular/router';

export const COMENSAL_ROUTES: Routes = [
  {
    path: 'ver-carta',
    loadComponent: () => import('./ver-carta/page/ver-carta').then(m => m.VerCartaComponent)
  },

];

