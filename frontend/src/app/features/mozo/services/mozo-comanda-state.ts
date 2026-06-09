import { computed, DestroyRef, effect, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MozoComandaService } from './mozo-comanda-service';
import { Comanda } from '../../../core/models/domain/comanda';
import { forkJoin } from 'rxjs';
import { ComandaHubService } from '../../../core/services/hubs/comanda/comanda-hub-service';
import { EstadoComanda, EstadoComandaId } from '../../../core/models/domain/comanda';

@Injectable({
  providedIn: 'root',
})
export class MozoComandaState {

  private api = inject(MozoComandaService);
  private hub = inject(ComandaHubService);
  private destroyRef = inject(DestroyRef);
  readonly #comandas = signal<Comanda[]>([]);
  readonly #cargando = signal<boolean>(false);

  comandas = this.#comandas.asReadonly();
  cargando = this.#cargando.asReadonly();

  comandasNuevas = computed(() =>
    this.#comandas().filter(c => c.estado === 'Nueva'));

  comandasEnPreparacion = computed(() =>
    this.#comandas().filter(c => c.estado === 'EnPreparacion'));

  comandasEnEspera = computed(() =>
    this.#comandas().filter(c => c.estado === 'EnEspera'));

  readonly #hubEffect = effect(() => {
    const actualizada = this.hub.comandaModificada();
    if (!actualizada) return;

    const normalizada = {
      ...actualizada,
      estado: (typeof actualizada.estado === 'string'
        ? actualizada.estado.replace(/\s/g, '')
        : EstadoComandaId[actualizada.estado as unknown as number]) as EstadoComanda
    };

    this.#comandas.update(lista => {
      const estadosFinales = ['Finalizada', 'Abierta'];
      if (estadosFinales.includes(normalizada.estado)) {
        return lista.filter(c => c.id !== normalizada.id);
      }
      const existe = lista.some(c => c.id === normalizada.id);
      if (existe) {
        return lista.map(c => c.id === normalizada.id ? normalizada : c);
      }
      return [normalizada, ...lista];
    });
  });

  cargarComandas(): void {
    this.#cargando.set(true);
    this.api.listarComandas().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (comandas) => {
        this.#comandas.set(comandas);

        this.#cargando.set(false);
      },
      error: (error) => {

        this.#cargando.set(false);
      }
    });
  }

  entregarItems(comandaId: number, articuloComandaIds: number[]): void {
    if (articuloComandaIds.length === 0) return;

    this.api.entregarItems(comandaId, articuloComandaIds).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (comandaActualizada) => {
        this.#comandas.update(lista =>
          lista.map(c => c.id === comandaActualizada.id ? comandaActualizada : c)
        );
      },
      error: (err) => {
        console.error('Error al entregar items:', err);
      }
    });
  }

  async conectarHub(restauranteId: number, mozoId: number): Promise<void> {
    await this.hub.conectarComoMozo(restauranteId, mozoId);
  }

  desconectarHub(): void {
    this.hub.desconectarEscucha();
  }
}
