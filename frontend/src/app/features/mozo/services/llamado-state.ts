import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Llamado } from '../../../core/models/llamados/llamado';
import { MozoHubService } from '../../../core/services/hubs/llamados/mozo-hub-service';
import { LlamadoService } from '../../../core/services/llamados/llamado-service';

@Injectable({
  providedIn: 'root',
})
export class LlamadoState { 
  readonly #api = inject(LlamadoService);
  readonly #hub = inject(MozoHubService);

  readonly #_llamados = signal<Llamado[]>([]);
  readonly #_cargando = signal<boolean>(true);
  readonly #_error = signal<string | null>(null);
  readonly #_resolviendoId = signal<number | null>(null);
  readonly #_hubConectado = signal<boolean>(false);

  #mozoId: number | null = null;
  #restauranteId: number | null = null;

  readonly llamados = this.#_llamados.asReadonly();
  readonly cargando = this.#_cargando.asReadonly();
  readonly error = this.#_error.asReadonly();
  readonly resolviendoId = this.#_resolviendoId.asReadonly();
  readonly hubConectado = this.#_hubConectado.asReadonly();

  readonly cantidadPendientes = computed<number>(() => this.#_llamados().length);
  readonly hayLlamados = computed<boolean>(() => this.#_llamados().length > 0);

 
  readonly #hubEffect = effect(() => {
    const nuevo = this.#hub.llamadoRecibido();
    if (!nuevo) return;
   
    
    console.log("Llamado recibido en el state: ", nuevo);

    this.#_llamados.update((lista) =>
      lista.some((l) => l.id === nuevo.id) ? lista : [nuevo, ...lista],
    );
  });

  cargar(mozoId: number, restauranteId: number): void {
    this.#mozoId = mozoId;
    this.#restauranteId = restauranteId;
    this.#_error.set(null);

    this.#_cargando.set(true);
    this.#api
      .listarPendientesDelMozo()
      .subscribe({
        next: (lista) => {
          this.#_llamados.set(lista);
          this.#_cargando.set(false);
          console.log("Llamados pendientes cargados: ", lista);
        },
        error: () => {
          this.#_error.set('No pudimos cargar los llamados. Reintentá.');
          this.#_cargando.set(false);
        },
      });
  }

  async conectarHub(): Promise<void> {
    if (this.#mozoId === null || this.#restauranteId === null) {
      this.#_error.set('Faltan IDs del mozo o restaurante.');
      return;
    }

    try {
      await this.#hub.conectarYUnirseGrupo(this.#restauranteId, this.#mozoId);
      this.#_hubConectado.set(true);
      console.log("Conectado al hub de llamados para mozoId=", this.#mozoId, " restauranteId=", this.#restauranteId);
    } catch {
      this.#_hubConectado.set(false);
      this.#_error.set('No pudimos conectar al sistema de notificaciones.');
    }
  }

  resolver(llamadoId: number): void {
    if (!llamadoId || this.#_resolviendoId() !== null) return;

    this.#_resolviendoId.set(llamadoId);

    this.#api
      .resolver(llamadoId)
      .subscribe({
        next: () => {
          this.#_llamados.update((lista) =>
            lista.filter((l) => l.id !== llamadoId),
          );
          this.#_resolviendoId.set(null);
        },
        error: () => {
          this.#_resolviendoId.set(null);
          this.#_error.set('No pudimos marcar el llamado como resuelto.');
        },
      });
  }

  reintentar(): void {
    if (this.#mozoId !== null && this.#restauranteId !== null) {
      this.cargar(this.#mozoId, this.#restauranteId);
    }
  }
}
