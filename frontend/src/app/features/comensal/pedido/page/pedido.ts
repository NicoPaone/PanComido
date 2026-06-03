import { Component, inject, computed } from '@angular/core';
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

  // Usar el signal del servicio directamente (reactivo)
  pedidos = this.pedidoService.pedidos;
  configuracion = configuracionRestauranteMock;

  // Computed para el total
  total = computed(() => {
    return this.pedidos().reduce(
      (acc, item) => acc + item.plato.precioVentaFinal * item.cantidad,
      0
    );
  });

  irADetallePedido(): void {
    this.router.navigate(['/comensal/detalle-pedido']);
  }

  volver(): void {
    this.router.navigate(['/comensal/ver-carta']);
  }

  eliminarPedido(index: number): void {
    this.pedidoService.eliminarPedido(index);
  }

  irAPersonalizar(item: ItemPedido, index: number): void {
    this.router.navigate(['/comensal/personalizar-plato'], {
      state: { plato: item, index }
    });
  }
}
