import { Injectable } from '@angular/core';
import { Plato } from '../models/plato';

@Injectable({
  providedIn: 'root'
})
export class PedidoService {

  pedidos: Plato[] = [];

  agregarPedido(plato: Plato) {
    this.pedidos.push(plato);
  }

  obtenerPedidos() {
    return this.pedidos;
  }

}