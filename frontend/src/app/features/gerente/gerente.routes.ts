import { Routes } from '@angular/router';

export const GERENTE_ROUTES: Routes = [
  { 
    path: 'stock-mercaderia',
    loadComponent: () => import('./stock-mercaderia/page/stock/stock').then(m => m.Stock)


    },

  {
    path: 'modificar-carta',
    loadComponent: () => import('./modificar-carta/page/modificar-carta').then(m => m.ModificarCartaComponent)
  },
  {
    path: 'crear-plato',
    loadComponent: () => import('./crear-plato/page/crear-plato').then(m => m.CrearPlatoComponent)
  }

];