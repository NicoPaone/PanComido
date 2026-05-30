import { App } from './../../../app';
import { Injectable, inject, signal } from '@angular/core';
import { MesaService } from '../../../core/services/mesa.service';
import { Mesa, EstadoMesa, FormaMesa } from '../../../core/models/mesa.model';

@Injectable({ providedIn: 'root' })
export class MesaStateService {
  private api = inject(MesaService);

  private _mesas = signal<Mesa[]>([]);
  private _loading = signal<boolean>(false);
  private _isEditorMode = signal<boolean>(false);
  private _mesasBackup: Mesa[] = [];
  private _notificacion = signal<{mensaje: string, tipo: 'exito' | 'error'} | null>(null);
  private _mesaSeleccionada = signal<number | null>(null);

  mesas = this._mesas.asReadonly();
  loading = this._loading.asReadonly();
  isEditorMode = this._isEditorMode.asReadonly();
  notificacion = this._notificacion.asReadonly();
  mesaSeleccionada = this._mesaSeleccionada.asReadonly();

  cargarMesas(): void {
    this._loading.set(true);
    this.api.getMesas().subscribe({
      next: (data) => {
        this._mesas.set(data);
        this._loading.set(false);
      },
      error: () => this._loading.set(false)
    });
  }

  mostrarNotificacion(mensaje: string, tipo: 'exito' | 'error'): void {
    this._notificacion.set({ mensaje, tipo });

    setTimeout(() => {
      this._notificacion.set(null);
    }, 3000);
  }

  toggleEditorMode(): void {
    const isEditor = this._isEditorMode();
    if (!isEditor) {
      this._mesasBackup = JSON.parse(JSON.stringify(this._mesas()));
    }
    this._mesaSeleccionada.set(null); // <-- Limpiamos menús abiertos
    this._isEditorMode.set(!isEditor);
  }

  cancelarEdicion(): void {
    this._mesas.set(JSON.parse(JSON.stringify(this._mesasBackup)));
    this._isEditorMode.set(false);
  }

  moverMesa(id: number, deltaX: number, deltaY: number): void {
    const mesaActual = this._mesas().find(m => m.id === id);
    if (!mesaActual) return;

    const nuevosDatos: Partial<Mesa> = {
      posicionXInicio: mesaActual.posicionXInicio + deltaX,
      posicionXfin: mesaActual.posicionXfin + deltaX,
      posicionYinicio: mesaActual.posicionYinicio + deltaY,
      posicionYFin: mesaActual.posicionYFin + deltaY
    };


    this._mesas.update(mesas =>
      mesas.map(m => m.id === id ? { ...m, ...nuevosDatos } : m)
    );
  }
  seleccionarMesa(id: number | null): void {
    if (this._isEditorMode()) return;

    this._mesaSeleccionada.update(actual => actual === id ? null : id);
  }
  agregarMesa(forma: FormaMesa): void {
    const mesas = this._mesas();
    const proximoNumero = mesas.length > 0 ? Math.max(...mesas.map(m => m.numeroMesa)) + 1 : 1;

    let ancho = 90;
    let alto = 90;

    switch (forma) {
      case FormaMesa.HorizontalLarga:
        ancho = 150;
        alto = 75;
        break;
      case FormaMesa.HorizontalAlta:
        ancho = 75;
        alto = 150;
        break;
      case FormaMesa.Redonda:
      case FormaMesa.Cuadrada:
      default:
        ancho = 90;
        alto = 90;
        break;
    }

    const nuevaMesa: Mesa = {
      id: Date.now(),
      codigoInvitacion: `MESA-TEMP`,
      numeroMesa: proximoNumero,
      cantidadPersonasMax: 4,
      estadoMesa: EstadoMesa.Disponible,
      dimensionMesa: { id: 0, forma, imagen: '' },
      posicionXInicio: 15,
      posicionXfin: 15 + ancho,
      posicionYinicio: 15,
      posicionYFin: 15 + alto
    };

    this._mesas.update(m => [...m, nuevaMesa]);
  }

  eliminarMesa(id: number): void {
    this._mesas.update(mesas => mesas.filter(m => m.id !== id));
  }

  cambiarEstadoMesa(id: number, nuevoEstado: EstadoMesa): void {
    this._mesas.update(mesas =>
      mesas.map(m => m.id === id ? { ...m, estadoMesa: nuevoEstado } : m)
    );

    this._mesaSeleccionada.set(null);
  }

  actualizarNumero(id: number, nuevoNumero: number): void {
    this._mesas.update(mesas =>
      mesas.map(m => m.id === id ? { ...m, numeroMesa: nuevoNumero } : m)
    );
  }

  guardarConfiguracion(): void {
    const mesas = this._mesas();

    const numeros = mesas.map(m => m.numeroMesa);
    if (new Set(numeros).size !== numeros.length) {
      this.mostrarNotificacion('Hay mesas con números duplicados. Corregilo antes de guardar.', 'error');
      return;
    }

    for (let i = 0; i < mesas.length; i++) {
      for (let j = i + 1; j < mesas.length; j++) {
        const mesaA = mesas[i];
        const mesaB = mesas[j];

        const seSolapan = !(
          mesaA.posicionXfin <= mesaB.posicionXInicio ||
          mesaA.posicionXInicio >= mesaB.posicionXfin ||
          mesaA.posicionYFin <= mesaB.posicionYinicio ||
          mesaA.posicionYinicio >= mesaB.posicionYFin
        );

        if (seSolapan) {
          this.mostrarNotificacion(`Las mesas ${mesaA.numeroMesa} y ${mesaB.numeroMesa} se están tocando.`, 'error');
          return;
        }
      }
    }

    console.log('JSON LISTO PARA MANDAR AL BACKEND CON PUT:', mesas);
    this._isEditorMode.set(false);
    this.mostrarNotificacion('Mapa guardado con éxito', 'exito');
  }
}
