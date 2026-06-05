import { DestroyRef, Injectable, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MesaService } from './mesa.service';
import { Mesa, EstadoMesa, FormaMesa } from '../../../core/models/domain/mesa';
import { MesaLecturaState } from '../shared/mesa-lectura-state';

@Injectable({ providedIn: 'root' })
export class MesaState {
  private lectura = inject(MesaLecturaState);
  private mesaService = inject(MesaService);
  private destroyRef = inject(DestroyRef);

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
  mostrarNotificacion(mensaje: string, tipo: 'exito' | 'error' | 'info'): void { this.lectura.mostrarNotificacion(mensaje, tipo); }

  // Métodos de EDICIÓN (solo gerente)
  toggleEditorMode(): void {
    if (!this._isEditorMode()) {
      this._mesasBackup = JSON.parse(JSON.stringify(this.lectura.mesas()));
    }
    this.lectura.seleccionarMesa(null);
    this._isEditorMode.set(!this._isEditorMode());
  }

  cancelarEdicion(): void {
    this.lectura.setMesas(JSON.parse(JSON.stringify(this._mesasBackup)));
    this._isEditorMode.set(false);
  }

  moverMesa(id: number, deltaX: number, deltaY: number): void {
    const mesas = this.lectura.mesas();
    const mesa = mesas.find(m => m.id === id);
    if (!mesa) return;

    this.lectura.updateMesas(mesas =>
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

    const idNegativo = -(Math.floor(Math.random() * 1000000) + 1);

    const nuevaMesa: Mesa = {
      id: idNegativo,
      codigoInvitacion: '',
      numeroMesa: proximoNumero,
      cantidadPersonasMax: 4,
      estadoMesa: EstadoMesa.Disponible,
      dimensionMesa: { id: 0, forma },
      posicionXInicio: 15, posicionXFin: 15 + ancho,
      posicionYInicio: 15, posicionYFin: 15 + alto
    };

    this.lectura.updateMesas(m => [...m, nuevaMesa]);
  }

  eliminarMesa(id: number): void {
    const mesas = this.lectura.mesas();
    const mesa = mesas.find(m => m.id === id);

    if (mesa && mesa.estadoMesa !== EstadoMesa.Disponible) {
      this.lectura.mostrarNotificacion('No se puede eliminar una mesa ocupada o con comandas activas', 'error');
      return;
    }

    this.lectura.updateMesas(mesas => mesas.filter(m => m.id !== id));
  }

  actualizarNumero(id: number, nuevoNumero: number): void {
    this.lectura.updateMesas(mesas =>
      mesas.map(m => m.id === id ? { ...m, numeroMesa: nuevoNumero } : m)
    );
  }

  guardarConfiguracion(): void {
    const mesas = this.lectura.mesas();

    // Validación de límites espaciales (máximo 3000px)
    const LIMITE = 3000;
    const mesaFueraDeRango = mesas.find(m =>
      m.posicionXInicio < 0 || m.posicionXFin > LIMITE ||
      m.posicionYInicio < 0 || m.posicionYFin > LIMITE
    );

    if (mesaFueraDeRango) {
      this.lectura.mostrarNotificacion(`La mesa ${mesaFueraDeRango.numeroMesa} está fuera del límite del mapa (${LIMITE}px)`, 'error');
      return;
    }

    // Validación de superposición
    for (let i = 0; i < mesas.length; i++) {
      for (let j = i + 1; j < mesas.length; j++) {
        const m1 = mesas[i];
        const m2 = mesas[j];

        const superponenX = m1.posicionXInicio < m2.posicionXFin && m1.posicionXFin > m2.posicionXInicio;
        const superponenY = m1.posicionYInicio < m2.posicionYFin && m1.posicionYFin > m2.posicionYInicio;

        if (superponenX && superponenY) {
          this.lectura.mostrarNotificacion(`Las mesas ${m1.numeroMesa} y ${m2.numeroMesa} están superpuestas. Por favor separalas.`, 'error');
          return;
        }
      }
    }

    this.mesaService.guardarMapa(mesas).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this._isEditorMode.set(false);
        this.lectura.mostrarNotificacion('Mapa guardado con éxito', 'exito');
      },
      error: (err) => {
        const mensaje = err.error?.error || 'Error al guardar el mapa';
        this.lectura.mostrarNotificacion(mensaje, 'error');
      }
    });
  }
}