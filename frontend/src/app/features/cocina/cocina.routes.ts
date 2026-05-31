import { Routes } from '@angular/router';

export const COCINA_ROUTES: Routes = [
  { 
    path: 'comandas',
    loadComponent: () => import('./comandas/pages/comanda-page/comanda-page').then(m => m.ComandaPage)
   },


];