import { Component, inject, computed , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { PedidoState } from '../../services/pedido.state';
import { ItemPedido } from '../../../../core/models/domain/item-pedido';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import {Boton} from '../../../../shared/ui/botones/boton/boton';
import { configuracionRestauranteMock } from '../../../../infra/mocks/configuracion-restaurante.mock-data';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { ComensalState } from '../../services/comensal-state';
import { ComandaState } from '../../services/comanda-state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-pedido',
  standalone: true,
  imports: [BotonComensal, Boton, LlamarAlMozo],
  templateUrl: './pedido.html'
})
export class Pedido {
  private router = inject(Router);
  private pedidoService = inject(PedidoState);
  comensalState = inject(ComensalState);
  comandaState = inject(ComandaState);

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

  agregarAlPedido(index: number): void {
  this.pedidoService.incrementarCantidad(index);
}

eliminarUno(index: number): void {
  this.pedidoService.decrementarCantidad(index);
}
}
