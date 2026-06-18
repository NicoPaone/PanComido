import { Component, inject, OnInit, ChangeDetectionStrategy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragEnd, CdkDragMove, DragDropModule } from '@angular/cdk/drag-drop';
import { MesaState } from '../../services/mesa.state';
import { EstadoMesa, FormaMesa, Mesa } from '../../../../core/models/domain/mesa';
import { MesaItem } from '../../../../shared/components/mesa-item/mesa-item';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-mapa-mesas',
  standalone: true,
  imports: [CommonModule, DragDropModule, MesaItem],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './mapa-mesas.html',
  styleUrl: './mapa-mesas.css'
})
export class MapaMesas implements OnInit {
  state = inject(MesaState);
  auth = inject(AuthService);
  FormaMesa = FormaMesa;
  mesaMobileSeleccionada = signal<Mesa | null>(null);

  // Modal de ocupar mesa
  mostrarModalOcupar = signal<boolean>(false);
  mesaSeleccionadaId = signal<number | null>(null);
  cantidadComensales = signal<number>(2);

  // Modal de comanda
  mostrarModalComanda = signal<boolean>(false);
  mesaComandaId = signal<number | null>(null);

  ngOnInit() {
    this.state.cargarMesas(); // Dispara la carga inicial al mock
  }

  seleccionarMesaMobile(mesa: Mesa) {
    this.mesaMobileSeleccionada.set(mesa);
  }

  volverGridMobile() {
    this.mesaMobileSeleccionada.set(null);
  }

  abrirMesaMobile() {
    if (this.mesaMobileSeleccionada()) {
      this.ejecutarAccion(this.mesaMobileSeleccionada()!.id, 'ocupar');
      this.mesaMobileSeleccionada.set(null); // Volvemos al grid automáticamente
    }
  }

  cerrarMesaMobile() {
    if (this.mesaMobileSeleccionada()) {
      this.ejecutarAccion(this.mesaMobileSeleccionada()!.id, 'cerrar');
      this.mesaMobileSeleccionada.set(null);
    }
  }

  habilitarMesaMobile() {
    if (this.mesaMobileSeleccionada()) {
      this.ejecutarAccion(this.mesaMobileSeleccionada()!.id, 'cerrar');
      this.mesaMobileSeleccionada.set(null);
    }
  }

  deshabilitarMesaMobile() {
    if (this.mesaMobileSeleccionada()) {
      this.ejecutarAccion(this.mesaMobileSeleccionada()!.id, 'deshabilitar');
      this.mesaMobileSeleccionada.set(null);
    }
  }

  verComandaMobile() {
    if (this.mesaMobileSeleccionada()) {
      this.state.mostrarNotificacion('Esperando comanda de los comensales...', 'info');
    }
  }

  gridSize = 15; // El tamaño de tu grilla en píxeles

  // Función que fuerza a la mesa a moverse visualmente en saltos de 15px


  // Mapa de colisiones activas
  colisionesActivas = signal<Record<number, boolean>>({});

  onDragMoved(id: number, event: CdkDragMove) {
    const mesaActual = this.state.mesas().find(m => m.id === id);
    if (!mesaActual) return;

    const x = mesaActual.posicionXInicio + event.distance.x;
    const y = mesaActual.posicionYInicio + event.distance.y;
    const ancho = mesaActual.posicionXFin - mesaActual.posicionXInicio;
    const alto = mesaActual.posicionYFin - mesaActual.posicionYInicio;

    const colision = this.state.mesas().some(otra => {
      if (otra.id === id) return false;
      const superponenX = x < otra.posicionXFin && (x + ancho) > otra.posicionXInicio;
      const superponenY = y < otra.posicionYFin && (y + alto) > otra.posicionYInicio;
      return superponenX && superponenY;
    });

    this.colisionesActivas.update(cols => ({ ...cols, [id]: colision }));
  }

  onDragEnded(id: number, event: CdkDragEnd) {
    const hayColision = this.colisionesActivas()[id];
    this.colisionesActivas.update(cols => ({ ...cols, [id]: false }));

    if (hayColision) {
      event.source._dragRef.reset();
      return;
    }

    // También ajustamos la matemática acá para que el Signal guarde el múltiplo exacto
    const deltaX = Math.round(event.distance.x / this.gridSize) * this.gridSize;
    const deltaY = Math.round(event.distance.y / this.gridSize) * this.gridSize;

    if (deltaX === 0 && deltaY === 0) return;

    event.source._dragRef.reset();
    this.state.moverMesa(id, deltaX, deltaY);
  }
  ejecutarAccion(id: number, accion: string) {
    switch (accion) {
      case 'ocupar':
        this.mesaSeleccionadaId.set(id);
        this.mostrarModalOcupar.set(true);
        break;
      case 'cerrar':
        this.state.cambiarEstadoMesa(id, EstadoMesa.Disponible);
        break;
      case 'deshabilitar':
        this.state.cambiarEstadoMesa(id, EstadoMesa.Deshabilitada);
        break;
      case 'detalles':
        this.state.mostrarNotificacion('Esperando pedido de los comensales...', 'info');
        this.state.seleccionarMesa(null);
        break;
    }
  }

  onMesaClick(id: number) {
    if (this.state.isEditorMode()) return;

    // Próximamente: Llamar al backend para traer la comanda activa por MesaId
    // Ej: GET /comanda/mesa/{id}/activa
    console.warn('onMesaClick no implementado aún');
  }

  confirmarOcupar() {
    const mesaId = this.mesaSeleccionadaId();
    const comensales = this.cantidadComensales();
    if (mesaId === null || comensales < 1) return;
    this.state.ocuparMesa(mesaId, comensales);
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

  mesasOrdenadas() {
    return [...this.state.mesas()]
      .filter(m => m.tipoElemento !== 2)
      .sort((a, b) => a.numeroMesa - b.numeroMesa);
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


}
