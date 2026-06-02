import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PedidoService } from '../../../../core/services/pedido.service';
import { ItemPedido } from '../../../../core/models/item-pedido';

@Component({
  selector: 'app-detalle-pedido',
  standalone: true,
  imports: [BotonComensal, LlamarAlMozo],
  templateUrl: './detalle-pedido.html',
  styleUrls: ['./detalle-pedido.css']
})
export class DetallePedido {
   private router = inject(Router);
  private pedidoService = inject(PedidoService);

  configuracion = configuracionRestauranteMock;
  pedidos = signal<ItemPedido[]>([]);

  constructor() {
    this.pedidos.set(this.pedidoService.obtenerPedidos());
  }

  get total(): number {
    return this.pedidos().reduce(
      (acc, item) => acc + item.plato.precio * item.cantidad,
      0
    );
  }

  volver(): void {
    this.router.navigate(['/comensal/pedido']);
  }

  confirmarPedido(): void {
    alert('Pedido confirmado');
  }
}
