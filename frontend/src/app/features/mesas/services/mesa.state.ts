import { App } from './../../../app';
import { Injectable, inject, signal } from '@angular/core';
import { MesaService } from '../../../core/services/mesa.service';
import { Mesa, EstadoMesa, FormaMesa } from '../../../core/models/mesa.model';
import { MesaLecturaState } from '../shared/mesa-lectura-state';

@Injectable({ providedIn: 'root' })
export class MesaStateService {
  private lectura = inject(MesaLecturaState);

  // Expone lo que ya tiene MesaLecturaState
  mesas = this.lectura.mesas;
  mesaSeleccionada = this.lectura.mesaSeleccionada;
  notificacion = this.lectura.notificacion;

  // Estado de edición (solo gerente)
  private _isEditorMode = signal<boolean>(false);
  private _mesasBackup: Mesa[] = [];
  isEditorMode = this._isEditorMode.asReadonly();

  // Métodos que delega a MesaLecturaState
  cargarMesas(): void { this.lectura.cargarMesas(); }
  seleccionarMesa(id: number | null): void { this.lectura.seleccionarMesa(id); }
  ocuparMesa(mesaId: number, comensales: number): void { this.lectura.ocuparMesa(mesaId, comensales); }
  cambiarEstadoMesa(id: number, estado: EstadoMesa): void { this.lectura.cambiarEstadoMesa(id, estado); }

  // Métodos de EDICIÓN (solo gerente)
  toggleEditorMode(): void {
    if (!this._isEditorMode()) {
      this._mesasBackup = JSON.parse(JSON.stringify(this.lectura.mesas()));
    }
    this.lectura.seleccionarMesa(null);
    this._isEditorMode.set(!this._isEditorMode());
  }

  cancelarEdicion(): void {
    this.lectura['_mesas'].set(JSON.parse(JSON.stringify(this._mesasBackup)));
    this._isEditorMode.set(false);
  }

  moverMesa(id: number, deltaX: number, deltaY: number): void {
    const mesas = this.lectura.mesas();
    const mesa = mesas.find(m => m.id === id);
    if (!mesa) return;

    this.lectura['_mesas'].update(mesas =>
      mesas.map(m => m.id === id ? {
        ...m,
        posicionXInicio: m.posicionXInicio + deltaX,
        posicionXFin: m.posicionXFin + deltaX,
        posicionYInicio: m.posicionYInicio + deltaY,
        posicionYFin: m.posicionYFin + deltaY
      } : m)
    );
  }

  agregarMesa(forma: FormaMesa): void {
    const mesas = this.lectura.mesas();
    const proximoNumero = mesas.length > 0 ? Math.max(...mesas.map(m => m.numeroMesa)) + 1 : 1;

    let ancho = 90, alto = 90;
    if (forma === FormaMesa.HorizontalLarga) { ancho = 150; alto = 75; }
    else if (forma === FormaMesa.HorizontalAlta) { ancho = 75; alto = 150; }

    const nuevaMesa: Mesa = {
      id: Date.now(), codigoInvitacion: '', numeroMesa: proximoNumero,
      cantidadPersonasMax: 4, estadoMesa: EstadoMesa.Disponible,
      dimensionMesa: { id: 0, forma },
      posicionXInicio: 15, posicionXFin: 15 + ancho,
      posicionYInicio: 15, posicionYFin: 15 + alto
    };

    this.lectura['_mesas'].update(m => [...m, nuevaMesa]);
  }

  eliminarMesa(id: number): void {
    this.lectura['_mesas'].update(mesas => mesas.filter(m => m.id !== id));
  }

  actualizarNumero(id: number, nuevoNumero: number): void {
    this.lectura['_mesas'].update(mesas =>
      mesas.map(m => m.id === id ? { ...m, numeroMesa: nuevoNumero } : m)
    );
  }

  guardarConfiguracion(): void {
    const mesas = this.lectura.mesas();
    const posiciones = mesas
      .filter(m => m.id < 1000000000)
      .map(m => ({
        mesaId: m.id,
        posicionXInicio: m.posicionXInicio,
        posicionXFin: m.posicionXFin,
        posicionYInicio: m.posicionYInicio,
        posicionYFin: m.posicionYFin
      }));

    if (posiciones.length > 0) {
      inject(MesaService).guardarPosiciones(posiciones).subscribe({
        next: () => {
          this._isEditorMode.set(false);
          this.lectura.mostrarNotificacion('Mapa guardado con éxito', 'exito');
        },
        error: () => this.lectura.mostrarNotificacion('Error al guardar', 'error')
      });
    }
  }
}