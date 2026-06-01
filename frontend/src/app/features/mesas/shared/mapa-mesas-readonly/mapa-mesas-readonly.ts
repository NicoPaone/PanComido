import { Component, inject, input, output } from '@angular/core';
import { MesaLecturaState } from '../mesa-lectura-state';
import { EstadoMesa } from '../../../../core/models/mesa.model';
import { MesaItem } from '../../../../shared/components/mesa-item/mesa-item';

@Component({
  selector: 'app-mapa-mesas-readonly',
  imports: [MesaItem],
  templateUrl: './mapa-mesas-readonly.html',
  styleUrl: './mapa-mesas-readonly.css',
})
export class MapaMesasReadonly {
   state = inject(MesaLecturaState);

  altura = input<string>('500px');

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
    switch (accion) {
      case 'ocupar':
        this.onOcuparMesa.emit(mesaId);
        this.state.seleccionarMesa(null);
        break;
      case 'detalles':
        this.onVerDetalles.emit(mesaId);
        this.state.seleccionarMesa(null);
        break;
      case 'cerrar':
        this.state.cambiarEstadoMesa(mesaId, EstadoMesa.Disponible);
        break;
      case 'deshabilitar':
        this.state.cambiarEstadoMesa(mesaId, EstadoMesa.Deshabilitada);
        break;
    }
  }
}
