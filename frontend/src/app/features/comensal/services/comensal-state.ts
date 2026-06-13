import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LlamadoService } from './llamado.service';
import { LlamadoMozo } from '../../../core/models/domain/llamado';
import { ComandaState } from './comanda-state';

@Injectable({
  providedIn: 'root',
})
export class ComensalState {
  readonly #api = inject(LlamadoService);
  readonly #comandaState = inject(ComandaState);
  readonly #destroyRef = inject(DestroyRef);
  readonly #_enviando = signal<boolean>(false);
  readonly #_exito = signal<boolean>(false);
  readonly #_error = signal<string | null>(null);

  readonly enviando = this.#_enviando.asReadonly();
  readonly exito = this.#_exito.asReadonly();
  readonly error = this.#_error.asReadonly();

  solicitarMozo(request: LlamadoMozo): void {
    const restauranteId = this.#comandaState.restauranteId();
    console.log("EL ID RESTAURANTE: ",restauranteId)
    if(!restauranteId) return;


    this.#_enviando.set(true);
    this.#_error.set(null);
    this.#_exito.set(false);

    this.#api.crearLlamado({...request,restauranteId}).pipe(takeUntilDestroyed(this.#destroyRef)).subscribe({
      next: () => {
        this.#_enviando.set(false);
        this.#_exito.set(true);
      },
      error: (e) => {
        this.#_enviando.set(false);
        this.#_error.set(e.error?.error ?? 'Error al solicitar mozo');
      }
    });    
  }

  limpiarEstado(): void {
    this.#_error.set(null);
    this.#_exito.set(false);
    this.#_enviando.set(false);
  }
}
