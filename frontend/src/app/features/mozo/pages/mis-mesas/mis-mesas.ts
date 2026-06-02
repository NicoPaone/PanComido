import { Component, inject, signal } from '@angular/core';
import { MapaMesasReadonly } from "../../../mesas/shared/mapa-mesas-readonly/mapa-mesas-readonly";
import { MesaLecturaState } from '../../../mesas/shared/mesa-lectura-state';

@Component({
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
    // Lógica cuando selecciona una mesa en el mapa
    console.log('Mesa seleccionada:', mesaId);
  }

  onOcuparMesa(mesaId: number) {
    console.log('Ocupar mesa:', mesaId);
    this.mesaSeleccionadaId.set(mesaId);
    this.mostrarModalOcupar.set(true);
  }

  onVerDetalles(mesaId: number) {
    this.mesaComandaId.set(mesaId);
    this.mostrarModalComanda.set(true);
  }

  confirmarOcupar() {
    const mesaId = this.mesaSeleccionadaId();
    const cantidadComensales = this.cantidadComensales();
    console.log('Confirmar ocupar mesa:', mesaId, 'para', cantidadComensales, 'comensales');
    if( mesaId === null || cantidadComensales < 1) return;
    this.mesaState.ocuparMesa(mesaId, cantidadComensales);
    console.log(`Ocupando mesa ${mesaId} para ${cantidadComensales} comensales`);

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
