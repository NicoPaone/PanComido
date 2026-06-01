import { Routes } from '@angular/router';

export const GERENTE_ROUTES: Routes = [
  {
    path: 'stock-mercaderia',
    loadComponent: () => import('./stock-mercaderia/page/insumo/insumo-page').then(m => m.InsumoPage)


  },
  {
    path: 'mapa-de-mesas',
    loadComponent: () => import('../mesas/components/mapa-mesas/mapa-mesas').then(m => m.MapaMesas)
  },
  {
    path: 'modificar-carta',
    loadComponent: () => import('./modificar-carta/page/modificar-carta').then(m => m.ModificarCartaComponent)
  }

];