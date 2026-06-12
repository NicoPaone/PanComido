import { Component, inject, input, output , ChangeDetectionStrategy} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MesaLecturaState } from '../mesa-lectura-state';
import { MesaItem } from '../../../../shared/components/mesa-item/mesa-item';
import { EstadoMesa, Mesa } from '../../../../core/models/domain/mesa';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-mapa-mesas-readonly',
  imports: [CommonModule, MesaItem],
  templateUrl: './mapa-mesas-readonly.html',
  styleUrl: './mapa-mesas-readonly.css',
})
export class MapaMesasReadonly {
  state = inject(MesaLecturaState);

  altura = input<string>('500px');
  mesaMobileSeleccionada: Mesa | null = null;

  onMesaSeleccionada = output<number>();
  onOcuparMesa = output<number>();
  onVerDetalles = output<number>();

  ngOnInit() {
    this.state.cargarMesas();
  }

  onMesaClick(id: number) {
    this.state.seleccionarMesa(id);
    this.onMesaSeleccionada.emit(id);
  }

  ejecutarAccion(mesaId: number, accion: string) {
    this.state.seleccionarMesa(null);

    switch (accion) {
      case 'ocupar':
        this.onOcuparMesa.emit(mesaId);
        break;
      case 'detalles':
        this.onVerDetalles.emit(mesaId);
        break;
      case 'cerrar':
        this.state.cambiarEstadoMesa(mesaId, EstadoMesa.Disponible);
        break;
      case 'deshabilitar':
        this.state.cambiarEstadoMesa(mesaId, EstadoMesa.Deshabilitada);
        break;
    }
  }

  mesasOrdenadas() {
    return [...this.state.mesas()].sort((a, b) => a.numeroMesa - b.numeroMesa);
  }

  getMobileClass(estado: string): string {
    switch (estado.toLowerCase()) {
      case 'disponible': return 'disponible';
      case 'ocupada': return 'ocupada';
      case 'reservada': return 'reservada';
      case 'deshabilitada': return 'deshabilitada';
      default: return 'disponible';
    }
  }

  seleccionarMesaMobile(mesa: Mesa) {
    this.mesaMobileSeleccionada = mesa;
  }

  volverGridMobile() {
    this.mesaMobileSeleccionada = null;
  }

  abrirMesaMobile() {
    if (this.mesaMobileSeleccionada) {
      this.ejecutarAccion(this.mesaMobileSeleccionada.id, 'ocupar');
      this.mesaMobileSeleccionada = null;
    }
  }

  cerrarMesaMobile() {
    if (this.mesaMobileSeleccionada) {
      this.ejecutarAccion(this.mesaMobileSeleccionada.id, 'cerrar');
      this.mesaMobileSeleccionada = null;
    }
  }

  deshabilitarMesaMobile() {
    if (this.mesaMobileSeleccionada) {
      this.ejecutarAccion(this.mesaMobileSeleccionada.id, 'deshabilitar');
      this.mesaMobileSeleccionada = null;
    }
  }

  habilitarMesaMobile() {
    if (this.mesaMobileSeleccionada) {
      this.ejecutarAccion(this.mesaMobileSeleccionada.id, 'cerrar');
      this.mesaMobileSeleccionada = null;
    }
  }

  verComandaMobile() {
    if (this.mesaMobileSeleccionada) {
      // this.ejecutarAccion(this.mesaMobileSeleccionada.id, 'detalles');
      this.state.mostrarNotificacion('Esperando pedido de los comensales...', 'info');
    }
  }

}
