
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PedidoService } from '../../../../core/services/pedido.service';
import { Component, OnInit } from '@angular/core';

interface PedidoItem {
  plato: {
    nombre: string;
    precioVenta: number;
  };
  cantidad: number;
}

@Component({
  selector: 'app-detalle-pedido',
  standalone: true,
  imports: [CommonModule, BotonComensal, LlamarAlMozo],
  templateUrl: './detalle-pedido.html',
    styleUrls: ['./detalle-pedido.css']
})
export class DetallePedido  implements OnInit{

configuracion = configuracionRestauranteMock;

 pedidos: PedidoItem[] = [];
  constructor(private router: Router, private pedidoService: PedidoService) {}

  get total(): number {
    return this.pedidos.reduce(
      (acc, item) => acc + item.plato.precioVenta * item.cantidad,
      0
    );
  }

ngOnInit() {
  this.pedidos = this.pedidoService.obtenerPedidos();
}

  volver() {
    this.router.navigate(['/comensal/pedido']);
  }

  editar(item: PedidoItem) {
    console.log('editar item:', item);
    // acá después podés navegar a personalizar-plato
    this.router.navigate(['/comensal/personalizar-plato']);
  }

  confirmarPedido() {
    console.log('pedido confirmado:', this.pedidos);
    alert('Pedido confirmado');
  }
}