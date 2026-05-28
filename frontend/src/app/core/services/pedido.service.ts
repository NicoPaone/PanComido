import { Injectable } from '@angular/core';

import { ItemPedido }
from '../models/item-pedido';

@Injectable({
  providedIn: 'root'
})
export class PedidoService {

  pedidos: ItemPedido[] = [];

  agregarPedido(item: ItemPedido) {

    this.pedidos.push(item);

  }

  obtenerPedidos() {

    return this.pedidos;

  }

  eliminarPedido(index: number) {

    this.pedidos.splice(index, 1);

  }

}