import { computed, DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EstadoMesa, Mesa, MesaOcupar } from '../../../core/models/domain/mesa';
import { MesaService } from '../services/mesa.service';

@Injectable({
  providedIn: 'root',
})
export class MesaLecturaState {
    private api = inject(MesaService);
    private destroyRef = inject(DestroyRef);

  private _mesas = signal<Mesa[]>([]);
  private _loading = signal<boolean>(false);
  private _mesaSeleccionada = signal<number | null>(null);
  private _notificacion = signal<{ mensaje: string; tipo: 'exito' | 'error' | 'info' } | null>(null);

  mesas = this._mesas.asReadonly();
  loading = this._loading.asReadonly();
  mesaSeleccionada = this._mesaSeleccionada.asReadonly();
  notificacion = this._notificacion.asReadonly();

  // Métodos de mutación seguros para evitar bypass de TypeScript
  setMesas(mesas: Mesa[]): void {
    this._mesas.set(mesas);
  }

  updateMesas(updater: (mesas: Mesa[]) => Mesa[]): void {
    this._mesas.update(updater);
  }

  mesasDisponibles = computed(() =>
    this._mesas().filter(m => m.estadoMesa === EstadoMesa.Disponible)
  );

  mesasOcupadas = computed(() =>
    this._mesas().filter(m => m.estadoMesa === EstadoMesa.Ocupada)
  );

  cargarMesas(): void {
    this._loading.set(true);
    this.api.getMesas().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => { this._mesas.set(data); this._loading.set(false); },
      error: () => this._loading.set(false)
    });
  }

  seleccionarMesa(id: number | null): void {
    this._mesaSeleccionada.update(actual => actual === id ? null : id);
  }
  ocuparMesa(mesaId:number,cantidadComensales:number):void{
    this.api.ocuparMesa(mesaId, cantidadComensales).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (response: MesaOcupar) => {
        this._mesas.update(mesas => mesas.map(m => m.id === mesaId ? response.mesa : m));
        this._mesaSeleccionada.set(null);
        this.mostrarNotificacion('Mesa ocupada exitosamente', 'exito');
      },
      error: () => this.mostrarNotificacion('Error al ocupar la mesa', 'error')
    });
  }

  cambiarEstadoMesa(id: number, nuevoEstado: EstadoMesa): void {
    this.api.cambiarEstado(id, nuevoEstado).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (mesaActualizada: Mesa) => {
        this._mesas.update(mesas =>
          mesas.map(m => m.id === id ? mesaActualizada : m)
        );
        this._mesaSeleccionada.set(null);
        this.mostrarNotificacion(
          `Mesa ${nuevoEstado === EstadoMesa.Ocupada ? 'ocupada' : 'actualizada'}`,
          'exito'
        );
      },
      error: () => this.mostrarNotificacion('Error al cambiar el estado de la mesa', 'error')
    });
  }

  mostrarNotificacion(mensaje: string, tipo: 'exito' | 'error' | 'info'): void {
    this._notificacion.set({ mensaje, tipo });
    setTimeout(() => this._notificacion.set(null), 3000);
  }
}
