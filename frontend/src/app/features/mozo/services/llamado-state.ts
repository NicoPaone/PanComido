import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { MozoHubService } from '../../../core/services/hubs/llamados/mozo-hub-service';
import { LlamadoService } from './llamado.service';
import { Llamado } from '../../../core/models/llamados/llamado';

const MS_NUEVO = 5000;
const MS_SALIDA = 400;

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
  readonly #_saliendoId = signal<number | null>(null);
  readonly #_hubConectado = signal<boolean>(false);
  readonly #_nuevos = signal<Set<number>>(new Set());

  #mozoId: number | null = null;
  #restauranteId: number | null = null;

  readonly llamados = this.#_llamados.asReadonly();
  readonly cargando = this.#_cargando.asReadonly();
  readonly error = this.#_error.asReadonly();
  readonly resolviendoId = this.#_resolviendoId.asReadonly();
  readonly saliendoId = this.#_saliendoId.asReadonly();
  readonly hubConectado = this.#_hubConectado.asReadonly();

  readonly cantidadPendientes = computed<number>(() => this.#_llamados().length);
  readonly hayLlamados = computed<boolean>(() => this.#_llamados().length > 0);

  esNuevo(id: number): boolean {
    return this.#_nuevos().has(id);
  }

  readonly #hubEffect = effect(() => {
    const nuevo = this.#hub.llamadoRecibido();
    if (!nuevo) return;

    this.#_llamados.update((lista) => {
      if (lista.some((l) => l.id === nuevo.id)) return lista;
      return [nuevo, ...lista];
    });

    this.#marcarNuevo(nuevo.id);
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
          const ordenados = [...lista].sort((a, b) => (b.id ?? 0) - (a.id ?? 0));
          this.#_llamados.set(ordenados);
          this.#_cargando.set(false);
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
    } catch {
      this.#_hubConectado.set(false);
      this.#_error.set('No pudimos conectar al sistema de notificaciones.');
    }
  }

  desconectarHub(): void {
    this.#hub.desconectarEscucha();
  }

  resolver(llamadoId: number): void {
    if (!llamadoId || this.#_resolviendoId() !== null) return;

    this.#_resolviendoId.set(llamadoId);

    this.#api.resolver(llamadoId)
      .subscribe({
        next: () => {
          this.#_resolviendoId.set(null);
          this.#_saliendoId.set(llamadoId);

          setTimeout(() => {
            this.#_llamados.update((lista) =>
              lista.filter((l) => l.id !== llamadoId),
            );
            this.#_nuevos.update((set) => {
              set.delete(llamadoId);
              return new Set(set);
            });
            this.#_saliendoId.set(null);
          }, MS_SALIDA);
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

  #marcarNuevo(id: number): void {
    this.#_nuevos.update((set) => new Set(set).add(id));
    setTimeout(() => {
      this.#_nuevos.update((set) => {
        set.delete(id);
        return new Set(set);
      });
    }, MS_NUEVO);
  }
}
