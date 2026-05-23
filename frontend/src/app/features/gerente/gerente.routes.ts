import { Routes } from '@angular/router';

export const GERENTE_ROUTES: Routes = [
<<<<<<< HEAD
  { 
    path: 'stock-mercaderia',
    loadComponent: () => import('./stock-mercaderia/page/stock/stock').then(m => m.Stock)
  
    //path: 'dashboard',
    //path"configuracion....

    },
  
=======
  {
    path: 'modificar-carta',
    loadComponent: () => import('./modificar-carta/page/modificar-carta').then(m => m.ModificarCartaComponent)
  }

>>>>>>> 233c997ba5d0860b7ee3801cd44172166b3be219
];