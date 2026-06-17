import { Routes } from '@angular/router';


export const COMENSAL_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'ver-carta', // cambiar a futuro a escanear-qr (este escaner debe servir tanto para mesa como fila virtual)
    pathMatch: 'full'
  },

  {
    path: 'seleccionar-mesa',
    loadComponent: () =>
      import('./seleccionar-mesa/page/seleccionar-mesa').then(m => m.SeleccionarMesa)
  },
  {
    path: 'escanear-mesa',
    loadComponent: () =>
      import('./escanear-mesa/page/escanear-mesa').then(m => m.ScanQr)
  },
  
  {
  path: 'cantidad-personas/:restauranteId/:mesaId',
  loadComponent: () =>
    import('./cantidad-personas/page/cantidad-personas')
      .then(m => m.CantidadPersonas)
},
  // {
  //   path: 'cantidad-personas',
  //   loadComponent: () =>
  //     import('./cantidad-personas/page/cantidad-personas').then(m => m.CantidadPersonas)
  // },
  // {
  //   path: 'ver-carta',
  //   loadComponent: () =>
  //     import('./ver-carta/page/ver-carta').then(m => m.VerCarta)
  // },
  {
  path: 'ver-carta/:restauranteId/:mesaId/:cantidadPersonas',
  loadComponent: () =>
    import('./ver-carta/page/ver-carta').then(m => m.VerCarta)
},
  {
    path: 'pedido',
    loadComponent: () =>
      import('./pedido/page/pedido')
        .then(m => m.Pedido)
  },
  {
    path: 'personalizar-plato',
    loadComponent: () =>
      import('./personalizar-plato/page/personalizar-plato')
        .then(m => m.PersonalizarPlato)
  },
  {
  path: 'detalle-pedido',
  loadComponent: () =>
    import('./detalle-pedido/page/detalle-pedido')
      .then(m => m.DetallePedido)
},
  {
    path: 'estado-pedido',
    loadComponent: () =>
      import('./estado-pedido/page/estado-pedido')
        .then(m => m.EstadoPedido)
  },
  {
    path: 'pago-checkout',
    loadComponent: () =>
      import('./pago-checkout/page/pago-checkout')
        .then(m => m.PagoCheckout)
  },
  {
    path: 'pago-confirmado',
    loadComponent: () =>
      import('./pago-confirmado/page/pago-confirmado')
        .then(m => m.PagoConfirmado)
  },
{
  path: 'mesa/:restauranteId/:mesaId',
  loadComponent: () =>
    import('./nro-de-mesa/page/nro-de-mesa').then(m => m.NroDeMesa)
},
{
  path: 'unirse/:comandaId',
  loadComponent: () =>
    import('./unirse-mesa/page/unirse-mesa').then(m => m.UnirseMesa)
}
];

