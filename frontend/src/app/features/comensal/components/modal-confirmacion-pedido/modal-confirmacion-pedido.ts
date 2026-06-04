import { Component, inject, input, output, signal , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { ComandaClienteResponse } from '../../../../core/models/comanda-cliente-response';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-modal-confirmacion-pedido',
  standalone: true,
  templateUrl: './modal-confirmacion-pedido.html',
  styleUrls: ['./modal-confirmacion-pedido.css']
})
export class ModalConfirmacionPedido {
  private router = inject(Router);

  cargando = input<boolean>(false);
  error = input<string | null>(null);
  estadoPedido = input<ComandaClienteResponse | null>(null);

  confirmar = output<void>();

  isVisible = signal(false);

  mostrar(): void {
    this.isVisible.set(true);
    this.confirmar.emit();
  }

  ocultar(): void {
    const huboExito = this.estadoPedido() !== null;
    this.isVisible.set(false);
    if (huboExito) {
      this.router.navigate(['/comensal/estado-pedido']);
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
