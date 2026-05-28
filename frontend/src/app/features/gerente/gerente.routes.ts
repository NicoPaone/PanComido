import { Routes } from '@angular/router';

export const GERENTE_ROUTES: Routes = [
  {
    path: 'stock-mercaderia',
    loadComponent: () => import('./stock-mercaderia/page/stock/stock').then(m => m.Stock)


    },
    {
   path: 'mapa-de-mesas',
    loadComponent: () => import('../mesas/page/mapa-mesas/mapa-mesas').then(m => m.MapaMesas)
  },
  {
    path: 'modificar-carta',
    loadComponent: () => import('./modificar-carta/page/modificar-carta').then(m => m.ModificarCartaComponent)
  },
  {
    path: 'ver-proveedores',
    loadComponent: () => import('./ver-proveedores/page/ver-proveedores').then(m => m.VerProveedoresComponent)
  },
  {
    path: 'nuevo-proveedor',
    loadComponent: () => import('./nuevo-proveedor/page/nuevo-proveedor').then(m => m.NuevoProveedorComponent)
  },
  {
    path: 'crear-plato',
    loadComponent: () => import('./crear-plato/page/crear-plato').then(m => m.CrearPlatoComponent)
  }

];
