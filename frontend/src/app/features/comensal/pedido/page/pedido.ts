import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PedidoService } from '../../../../core/services/pedido.service';
import { Plato } from '../../../../core/models/plato';
import { Router } from '@angular/router';
import { Boton } from '../../../../shared/ui/botones/boton/boton';

@Component({
  selector: 'app-pedido',
  standalone: true,
  imports: [CommonModule, Boton],
  templateUrl: './pedido.html',
  styleUrls: ['./pedido.css']
})
export class PedidoComponent implements OnInit {

  pedidos: Plato[] = [];

 constructor(
  private pedidoService: PedidoService,
  private router: Router
) {}

volver() {
  this.router.navigate(['/comensal/ver-carta']);
}

  ngOnInit() {
    this.pedidos = this.pedidoService.obtenerPedidos();
  }

 eliminarPedido(index: number) {

  this.pedidoService.eliminarPedido(index);

  this.pedidos = this.pedidoService.obtenerPedidos();

}

}