import { Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';

import { PedidoService }
from '../../../../core/services/pedido.service';

import { ItemPedido }
from '../../../../core/models/item-pedido';

import { Boton }
from '../../../../shared/ui/botones/boton/boton';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';



@Component({
  selector: 'app-pedido',
  standalone: true,
  imports: [
    CommonModule,
    Boton,
    BotonComensal
  ],
  templateUrl: './pedido.html',
  styleUrls: ['./pedido.css']
})
export class PedidoComponent
implements OnInit {

  pedidos: ItemPedido[] = [];
configuracion = configuracionRestauranteMock;

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

  irAPersonalizar(item: any) {

  this.router.navigate(
    ['/comensal/personalizar-plato'],
    {
      state: {
        plato: item
      }
    }
  );

}

}