import { computed, inject, Injectable, signal } from '@angular/core';
import { EstadoMesa, Mesa } from '../../../core/models/mesa.model';
import { MesaService } from '../../../core/services/mesa.service';

@Injectable({
  providedIn: 'root',
})
export class MesaLecturaState {
    private api = inject(MesaService);

  private _mesas = signal<Mesa[]>([]);
  private _loading = signal<boolean>(false);
  private _mesaSeleccionada = signal<number | null>(null);
  private _notificacion = signal<{ mensaje: string; tipo: 'exito' | 'error' } | null>(null);

  mesas = this._mesas.asReadonly();
  loading = this._loading.asReadonly();
  mesaSeleccionada = this._mesaSeleccionada.asReadonly();
  notificacion = this._notificacion.asReadonly();

  mesasDisponibles = computed(() =>
    this._mesas().filter(m => m.estadoMesa === EstadoMesa.Disponible)
  );

  mesasOcupadas = computed(() =>
    this._mesas().filter(m => m.estadoMesa === EstadoMesa.Ocupada)
  );

  cargarMesas(): void {
    this._loading.set(true);
    this.api.getMesas().subscribe({
      next: (data) => { this._mesas.set(data); this._loading.set(false); },
      error: () => this._loading.set(false)
    });
  }

  seleccionarMesa(id: number | null): void {
    this._mesaSeleccionada.update(actual => actual === id ? null : id);
  }

  cambiarEstadoMesa(id: number, nuevoEstado: EstadoMesa): void {
    this._mesas.update(mesas =>
      mesas.map(m => m.id === id ? { ...m, estadoMesa: nuevoEstado } : m)
    );
    this._mesaSeleccionada.set(null);
    this.mostrarNotificacion(
      `Mesa ${nuevoEstado === EstadoMesa.Ocupada ? 'ocupada' : 'cerrada'}`,
      'exito'
    );
  }

  mostrarNotificacion(mensaje: string, tipo: 'exito' | 'error'): void {
    this._notificacion.set({ mensaje, tipo });
    setTimeout(() => this._notificacion.set(null), 3000);
  }
}
