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
  private _comandas = signal<Comanda[]>([]);
  private _cargando = signal<boolean>(false);

  comandas = this._comandas.asReadonly();
  cargando = this._cargando.asReadonly();


  comandasNuevas = computed(() =>
    this._comandas().filter(c => c.estado === 'Nueva')
  );

  comandasEnPreparacion = computed(() =>
    this._comandas().filter(c => c.estado === 'EnPreparacion')
  );

  comandasEnEspera = computed(() =>
    this._comandas().filter(c => c.estado === 'EnEspera')
  );
  comandasfinalizadas = computed(() =>
    this._comandas().filter(c => c.estado === 'Finalizada')
  );


  modificarEstadoComanda(comandaId: number, tipoId: number): void {
    this.api.modificarEstadoComanda(comandaId, tipoId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (comandaActualizada) => {
        this._comandas.update(lista =>
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
        this._comandas.update(lista =>
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

    this._comandas.update(listaActual => {
      const existe = listaActual.some(comanda => comanda.id === normalizada.id);
      if (existe) {
        return listaActual.map(comanda => comanda.id === normalizada.id ? normalizada : comanda);
      } else {
        return [...listaActual, normalizada];
      }
    })
  }

  cargarComandasActivas(): void {
    this._cargando.set(true);
    this.api.obtenerComandasActivas().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => {
        this._comandas.set(data);
        this._cargando.set(false);
      },
      error: () => this._cargando.set(false)
    })
  }

}
