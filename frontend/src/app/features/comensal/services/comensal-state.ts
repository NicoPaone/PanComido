import { inject, Injectable, signal } from '@angular/core';
import { LlamadoService } from '../../../core/services/llamados/llamado-service';
import { LlamarMozoRequest } from '../../../core/models/llamados/llamado';

@Injectable({
  providedIn: 'root',
})
export class ComensalState {
  readonly #api = inject(LlamadoService);
  readonly #_llamadoEnviado = signal<boolean>(false);
  readonly #_exito = signal<boolean>(false);
  readonly #_error = signal<string | null>(null);
  readonly llamadoEnviado = this.#_llamadoEnviado.asReadonly();
  readonly exito = this.#_exito.asReadonly();
  readonly error = this.#_error.asReadonly();

  solicitarMozo(request: LlamarMozoRequest): void {
    this.#api.crearLlamado(request).subscribe({
      next: () => {
        this.#_llamadoEnviado.set(true);
        console.log('Mozo solicitado');
        this.#_exito.set(true);
      },
      error: (e) => {
        console.error('Error', e);
        this.#_error.set('Error al solicitar mozo');
      }
    });    

  }
  limpiarEstado(): void {
    this.#_error.set(null);
    this.#_exito.set(false);
  }
   
}

