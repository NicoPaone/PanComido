import { Component, inject, signal , ChangeDetectionStrategy} from '@angular/core';
import { MapaMesasReadonly } from "../../../mesas/shared/mapa-mesas-readonly/mapa-mesas-readonly";
import { MesaLecturaState } from '../../../mesas/shared/mesa-lectura-state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-mis-mesas',
  imports: [MapaMesasReadonly],
  templateUrl: './mis-mesas.html',
  styleUrl: './mis-mesas.css',
})
export class MisMesasPage {


  private mesaState = inject(MesaLecturaState);
  // Modal de ocupar mesa
  mostrarModalOcupar = signal<boolean>(false);
  mesaSeleccionadaId = signal<number | null>(null);
  cantidadComensales = signal<number>(2);

  // Modal de comanda
  mostrarModalComanda = signal<boolean>(false);
  mesaComandaId = signal<number | null>(null);

  onMesaSeleccionada(mesaId: number) {
    console.warn('onMesaSeleccionada no implementado aún');
  }

  onOcuparMesa(mesaId: number) {
    this.mesaSeleccionadaId.set(mesaId);
    this.mostrarModalOcupar.set(true);
  }

  onVerDetalles(mesaId: number) {
    this.mesaState.mostrarNotificacion('Esperando pedido de los comensales...', 'info');
  }

  confirmarOcupar() {
    const mesaId = this.mesaSeleccionadaId();
    const cantidadComensales = this.cantidadComensales();
    if (mesaId === null || cantidadComensales < 1) return;
    this.mesaState.ocuparMesa(mesaId, cantidadComensales);

    this.cerrarModalOcupar();
  }

  cerrarModalOcupar() {
    this.mostrarModalOcupar.set(false);
    this.mesaSeleccionadaId.set(null);
    this.cantidadComensales.set(2);
  }

  cerrarModalComanda() {
    this.mostrarModalComanda.set(false);
    this.mesaComandaId.set(null);
  }
}
