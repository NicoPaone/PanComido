import { Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';

import { PedidoService }
from '../../../../core/services/pedido.service';

import { ItemPedido }
from '../../../../core/models/item-pedido';

import { Boton }
from '../../../../shared/ui/botones/boton/boton';

@Component({
  selector: 'app-pedido',
  standalone: true,
  imports: [
    CommonModule,
    Boton
  ],
  templateUrl: './pedido.html',
  styleUrls: ['./pedido.css']
})
export class PedidoComponent
implements OnInit {

  pedidos: ItemPedido[] = [];

  constructor(

    private pedidoService:
      PedidoService,

    private router: Router

  ) {}

  ngOnInit() {

    this.pedidos =
      this.pedidoService.obtenerPedidos();

  }

  volver() {

    this.router.navigate([
      '/comensal/ver-carta'
    ]);

  }

  eliminarPedido(index: number) {

    this.pedidoService
      .eliminarPedido(index);

    this.pedidos =
      this.pedidoService.obtenerPedidos();

  }

  get total() {

    return this.pedidos.reduce(

      (acc, item) =>

        acc +

        (
          item.plato.precioVenta *
          item.cantidad
        ),

      0

    );

  }

}