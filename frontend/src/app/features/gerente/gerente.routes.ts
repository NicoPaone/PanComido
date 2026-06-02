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
    loadComponent: () => import('./modificar-carta/pages/modificar-carta').then(m => m.ModificarCartaComponent)
  },
  {
    path: 'ver-proveedores',
    loadComponent: () => import('./ver-proveedores/pages/ver-proveedores').then(m => m.VerProveedoresComponent)
  },
  {
    path: 'ver-proveedores/:id/historial',
    loadComponent: () => import('./ver-proveedores/pages/historial-proveedor/historial-proveedor').then(m => m.HistorialProveedorComponent)
  },
  {
    path: 'nuevo-proveedor',
    loadComponent: () => import('./nuevo-proveedor/pages/nuevo-proveedor').then(m => m.NuevoProveedorComponent)
  },
  {
    path: 'crear-plato',
    loadComponent: () => import('./crear-plato/pages/crear-plato').then(m => m.CrearPlatoComponent)
  },
  {
    path: 'realizar-pedido-sugerido',
    loadComponent: () => import('./realizar-pedido-sugerido/pages/realizar-pedido-sugerido').then(m => m.RealizarPedidoSugeridoComponent)
  },
  {
    path: 'realizar-pedido-sugerido/:id',
    loadComponent: () => import('./realizar-pedido-sugerido/pages/realizar-pedido-sugerido').then(m => m.RealizarPedidoSugeridoComponent)
  },
  {
    path: 'avisos',
    loadComponent: () => import('./avisos/pages/avisos').then(m => m.AvisosPage)
  },
  {
    path: 'aviso-vencimientos',
    loadComponent: () => import('./aviso-vencimientos/pages/aviso-vencimientos').then(m => m.VencimientosPage)
  }
];
