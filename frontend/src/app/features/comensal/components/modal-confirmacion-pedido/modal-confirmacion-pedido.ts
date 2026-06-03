import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ComandaStateService } from '../../services/comanda-state.service';
import { ComandaClienteResponse } from '../../../../core/models/comanda-cliente-response';

@Component({
  selector: 'app-modal-confirmacion-pedido',
  standalone: true,
  templateUrl: './modal-confirmacion-pedido.html',
  styleUrls: ['./modal-confirmacion-pedido.css']
})
export class ModalConfirmacionPedido {
  private router = inject(Router);
  comandaState = inject(ComandaStateService);

  isVisible = signal(false);
  estadoPedido = signal<ComandaClienteResponse | null>(null);

  mostrar(): void {
    this.isVisible.set(true);
    this.confirmarPedidoYActualizarEstado();
  }

  ocultar(): void {
    this.isVisible.set(false);
  }

  private async confirmarPedidoYActualizarEstado(): Promise<void> {
    try {
      const response = await this.comandaState.confirmarPedido();
      this.estadoPedido.set(response);
    } catch (error) {
      console.error('Error en confirmación:', error);
    }
  }

  verEstado(): void {
    this.router.navigate(['/comensal/estado-pedido']);
    this.ocultar();
  }

  getEstadoClase(): string {
    const estadoUI = this.estadoPedido()?.estadoUI;
    
    if (estadoUI === 'Recibido') return 'estado-recibido';
    if (estadoUI === 'Preparación') return 'estado-preparacion';
    if (estadoUI === 'Listo') return 'estado-listo';
    
    return 'estado-recibido';
  }

  getEstadoTexto(): string {
    const estadoUI = this.estadoPedido()?.estadoUI;
    
    if (estadoUI === 'Recibido') return 'Llegó a la cocina, en breve lo empiezan a preparar';
    if (estadoUI === 'Preparación') return 'El cocinero ya está trabajando en tu pedido';
    if (estadoUI === 'Listo') return 'Tu comida está terminada y en camino a la mesa';
    
    return 'Procesando tu pedido...';
  }

  isEstadoActivo(estado: string): boolean {
    const estadoUI = this.estadoPedido()?.estadoUI;
    return estadoUI === estado;
  }
}
