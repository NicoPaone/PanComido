import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { PedidoService } from '../../../../core/services/pedido.service';
import { ItemPedido } from '../../../../core/models/item-pedido';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';

@Component({
  selector: 'app-pedido',
  standalone: true,
  imports: [BotonComensal, LlamarAlMozo],
  templateUrl: './pedido.html'
})
export class Pedido {
  private router = inject(Router);
  private pedidoService = inject(PedidoService);

  pedidos = signal<ItemPedido[]>([]);
  configuracion = configuracionRestauranteMock;

  constructor() {
    this.pedidos.set(this.pedidoService.obtenerPedidos());
  }

  irADetallePedido(): void {
    this.router.navigate(['/comensal/detalle-pedido']);
  }

  volver(): void {
    this.router.navigate(['/comensal/ver-carta']);
  }

  eliminarPedido(index: number): void {
    this.pedidoService.eliminarPedido(index);
    this.pedidos.set(this.pedidoService.obtenerPedidos());
  }

  get total(): number {
    return this.pedidos().reduce(
      (acc, item) => acc + item.plato.precioVenta * item.cantidad,
      0
    );
  }

  irAPersonalizar(item: ItemPedido): void {
    this.router.navigate(['/comensal/personalizar-plato'], {
      state: { plato: item }
    });
  }
}
