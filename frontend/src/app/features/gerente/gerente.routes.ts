import { Routes } from '@angular/router';

export const GERENTE_ROUTES: Routes = [
  { 
    path: 'stock-mercaderia',
    loadComponent: () => import('./stock-mercaderia/page/insumo/insumo-page').then(m => m.InsumoPage)


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
    path: 'nuevo-proveedor',
    loadComponent: () => import('./nuevo-proveedor/pages/nuevo-proveedor').then(m => m.NuevoProveedorComponent)
  },
  {
    path: 'crear-plato',
    loadComponent: () => import('./crear-plato/pages/crear-plato').then(m => m.CrearPlatoComponent)
  },
  {
    path: 'pedido-sugerido-ia/:id',
    loadComponent: () => import('./pedido-sugerido-ia/pages/pedido-sugerido-ia').then(m => m.PedidoSugeridoIAComponent)
  }
  ,
  {
    path: 'avisos',
    loadComponent: () => import('./avisos/pages/avisos').then(m => m.AvisosPage)
  },
  {
    path: 'aviso-vencimientos',
    loadComponent: () => import('./aviso-vencimientos/pages/aviso-vencimientos').then(m => m.VencimientosPage)
  }
];