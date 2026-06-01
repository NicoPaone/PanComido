import { Component, signal } from '@angular/core';
import { MapaMesasReadonly } from '../../../mesas/shared/mapa-mesas-readonly/mapa-mesas-readonly';

@Component({
  selector: 'app-mis-mesas-page',
  imports: [MapaMesasReadonly],
  templateUrl: './mis-mesas-page.html',
  styleUrl: './mis-mesas-page.css',
})
export class MisMesasPage {
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
    this.mesaSeleccionadaId.set(mesaId);
    this.mostrarModalOcupar.set(true);
  }

  onVerDetalles(mesaId: number) {
    this.mesaComandaId.set(mesaId);
    this.mostrarModalComanda.set(true);
  }

  confirmarOcupar() {
    console.log('Ocupar mesa:', this.mesaSeleccionadaId(), 'Comensales:', this.cantidadComensales());
    // TODO: Llamar al backend para ocupar la mesa
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
