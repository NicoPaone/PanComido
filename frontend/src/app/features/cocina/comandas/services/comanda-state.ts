import { computed, DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ComandaService } from './comanda-service';
import { Comanda, EstadoComandaId, EstadoComanda } from '../../../../core/models/domain/comanda';

@Injectable({
  providedIn: 'root',
})
export class ComandaState {

  private api = inject(ComandaService);
  private destroyRef = inject(DestroyRef);
  readonly #comandas = signal<Comanda[]>([]);
  readonly #cargando = signal<boolean>(false);

  comandas = this.#comandas.asReadonly();
  cargando = this.#cargando.asReadonly();


  comandasNuevas = computed(() =>
    this.#comandas().filter(c => c.estado === 'Nueva')
  );

  comandasEnPreparacion = computed(() =>
    this.#comandas().filter(c => c.estado === 'EnPreparacion')
  );

  comandasEnEspera = computed(() =>
    this.#comandas().filter(c => c.estado === 'EnEspera')
  );
  comandasfinalizadas = computed(() =>
    this.#comandas().filter(c => c.estado === 'Finalizada')
  );


  modificarEstadoComanda(comandaId: number, tipoId: number): void {
    this.api.modificarEstadoComanda(comandaId, tipoId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (comandaActualizada) => {
        this.#comandas.update(lista =>
          lista.map(c => c.id === comandaActualizada.id
            ? comandaActualizada
            : c
          )
        );
      },
      error: (err) => console.error('Error al modificar comanda', err)
    });
  }
  marcarItemEntregado(comandaId: number, articuloComandaId: number): void {
    this.api.marcarItemEntregado(comandaId, articuloComandaId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (comandaActualizada) => {
        this.#comandas.update(lista =>
          lista.map(c => c.id === comandaActualizada.id ? comandaActualizada : c)
        );
      },
      error: (err) => console.error('Error al marcar item', err)
    });
  }


  actualizarDesdeHub(comandaRecibida: Comanda): void {
    const normalizada = {
      ...comandaRecibida,
      estado: (typeof comandaRecibida.estado === 'string'
        ? comandaRecibida.estado.replace(/\s/g, '')
        : EstadoComandaId[comandaRecibida.estado as unknown as number]) as EstadoComanda
    };

    this.#comandas.update(listaActual => {
      const existe = listaActual.some(comanda => comanda.id === normalizada.id);
      if (existe) {
        return listaActual.map(comanda => comanda.id === normalizada.id ? normalizada : comanda);
      } else {
        return [...listaActual, normalizada];
      }
    })
  }

  cargarComandasActivas(): void {
    this.#cargando.set(true);
    this.api.obtenerComandasActivas().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => {
        this.#comandas.set(data);
        this.#cargando.set(false);
      },
      error: () => this.#cargando.set(false)
    })
  }

}
