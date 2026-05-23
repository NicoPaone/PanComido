import { Routes } from '@angular/router';

export const GERENTE_ROUTES: Routes = [
  { 
    path: 'stock-mercaderia',
    loadComponent: () => import('./stock-mercaderia/page/stock/stock').then(m => m.Stock)
  
    //path: 'dashboard',
    //path"configuracion....

    },
  
];