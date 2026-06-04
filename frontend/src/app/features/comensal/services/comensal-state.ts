import { inject, Injectable, signal } from '@angular/core';
import { LlamadoService } from './llamado.service';
import { LlamarMozoRequest } from '../../../core/models/llamados/llamado';

@Injectable({
  providedIn: 'root',
})
export class ComensalState {
  readonly #api = inject(LlamadoService);
  readonly #_enviando = signal<boolean>(false);
  readonly #_exito = signal<boolean>(false);
  readonly #_error = signal<string | null>(null);

  readonly enviando = this.#_enviando.asReadonly();
  readonly exito = this.#_exito.asReadonly();
  readonly error = this.#_error.asReadonly();

  solicitarMozo(request: LlamarMozoRequest): void {
    this.#_enviando.set(true);
    this.#_error.set(null);
    this.#_exito.set(false);

    this.#api.crearLlamado(request).subscribe({
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
