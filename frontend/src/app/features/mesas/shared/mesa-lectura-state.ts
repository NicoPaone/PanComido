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

  readonly #mesas = signal<Mesa[]>([]);
  readonly #loading = signal<boolean>(false);
  readonly #mesaSeleccionada = signal<number | null>(null);
  readonly #notificacion = signal<{ mensaje: string; tipo: 'exito' | 'error' | 'info' } | null>(null);

  mesas = this.#mesas.asReadonly();
  loading = this.#loading.asReadonly();
  mesaSeleccionada = this.#mesaSeleccionada.asReadonly();
  notificacion = this.#notificacion.asReadonly();

  // Métodos de mutación seguros para evitar bypass de TypeScript
  setMesas(mesas: Mesa[]): void {
    this.#mesas.set(mesas);
  }

  updateMesas(updater: (mesas: Mesa[]) => Mesa[]): void {
    this.#mesas.update(updater);
  }

  mesasDisponibles = computed(() =>
    this.#mesas().filter(m => m.estadoMesa === EstadoMesa.Disponible)
  );

  mesasOcupadas = computed(() =>
    this.#mesas().filter(m => m.estadoMesa === EstadoMesa.Ocupada)
  );

  cargarMesas(): void {
    this.#loading.set(true);
    this.api.getMesas().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => { this.#mesas.set(data); this.#loading.set(false); },
      error: () => this.#loading.set(false)
    });
  }

  seleccionarMesa(id: number | null): void {
    this.#mesaSeleccionada.update(actual => actual === id ? null : id);
  }
  ocuparMesa(mesaId:number,cantidadComensales:number):void{
    this.api.ocuparMesa(mesaId, cantidadComensales).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (response: MesaOcupar) => {
        this.#mesas.update(mesas => mesas.map(m => m.id === mesaId ? response.mesa : m));
        this.#mesaSeleccionada.set(null);
        this.mostrarNotificacion('Mesa ocupada exitosamente', 'exito');
      },
      error: () => this.mostrarNotificacion('Error al ocupar la mesa', 'error')
    });
  }

  cambiarEstadoMesa(id: number, nuevoEstado: EstadoMesa): void {
    this.api.cambiarEstado(id, nuevoEstado).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (mesaActualizada: Mesa) => {
        this.#mesas.update(mesas =>
          mesas.map(m => m.id === id ? mesaActualizada : m)
        );
        this.#mesaSeleccionada.set(null);
        this.mostrarNotificacion(
          `Mesa ${nuevoEstado === EstadoMesa.Ocupada ? 'ocupada' : 'actualizada'}`,
          'exito'
        );
      },
      error: () => this.mostrarNotificacion('Error al cambiar el estado de la mesa', 'error')
    });
  }

  mostrarNotificacion(mensaje: string, tipo: 'exito' | 'error' | 'info'): void {
    this.#notificacion.set({ mensaje, tipo });
    setTimeout(() => this.#notificacion.set(null), 3000);
  }
}
