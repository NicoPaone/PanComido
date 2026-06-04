import { Component, inject, computed, ViewChild , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PedidoState } from '../../services/pedido.state';
import { ComandaState } from '../../services/comanda-state';
import { ModalConfirmacionPedido } from '../../components/modal-confirmacion-pedido/modal-confirmacion-pedido';
import { ComensalState } from '../../services/comensal-state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-detalle-pedido',
  standalone: true,
  imports: [LlamarAlMozo, ModalConfirmacionPedido],
  templateUrl: './detalle-pedido.html',
  styleUrls: ['./detalle-pedido.css']
})
export class DetallePedido {
  private router = inject(Router);
  private pedidoService = inject(PedidoState);
  comandaState = inject(ComandaState);
  comensalState = inject(ComensalState);

  @ViewChild(ModalConfirmacionPedido) modal!: ModalConfirmacionPedido;

  configuracion = configuracionRestauranteMock;
  
  // Usar el signal del servicio directamente (reactivo)
  pedidos = this.pedidoService.pedidos;

  // Computed para el total
  total = computed(() => {
    const totalCarrito = this.pedidos().reduce(
      (acc, item) => acc + item.plato.precioVentaFinal * item.cantidad,
      0
    );
    const totalConfirmado = this.comandaState.estadoPedido()?.totalAPagar || 0;
    return totalCarrito + totalConfirmado;
  });

  volver(): void {
    this.router.navigate(['/comensal/pedido']);
  }

  confirmarPedido(): void {
    // Validación: debe haber comanda activa
    if (!this.comandaState.tieneComandaActiva()) {
      alert('No hay mesa seleccionada. Por favor, escanea el QR de la mesa.');
      return;
    }

    // Validación: debe haber items en el carrito
    if (this.pedidos().length === 0) {
      alert('El carrito está vacío');
      return;
    }

    // Mostrar modal con confirmación
    this.modal.mostrar();
  }
}
