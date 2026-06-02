import { Injectable } from '@angular/core';

import { ItemPedido }
from '../models/item-pedido';

@Injectable({
  providedIn: 'root'
})
export class PedidoService {

  pedidos: ItemPedido[] = [];

  // agregarPedido(item: ItemPedido) {

  //   this.pedidos.push(item);

  // }

agregarPedido(item: ItemPedido) {

  console.log('Agregando:', item.plato.nombre);

  this.pedidos.push(item);

  console.log(
    'Pedidos actuales:',
    this.pedidos.map(p => p.plato.nombre)
  );

}

  obtenerPedidos() {

    return this.pedidos;

  }

  eliminarPedido(index: number) {

    this.pedidos.splice(index, 1);

  }

}