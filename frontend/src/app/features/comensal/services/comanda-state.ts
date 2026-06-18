import { DestroyRef, inject, Injectable, signal, computed, effect } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, tap, catchError, throwError, map } from 'rxjs';
import { ComandaService } from './comanda.service';
import { PedidoState } from './pedido.state';
import { EstadoPedido } from '../../../core/models/domain/comanda';
import { Mesa } from '../../../core/models/domain/mesa';
import { ComandaHubService } from '../../../core/services/hubs/comanda/comanda-hub-service';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ComandaState {
  private comandaService = inject(ComandaService);
  private pedidoService = inject(PedidoState);
  private http = inject(HttpClient);
  private comandaHub = inject(ComandaHubService);
  readonly #destroyRef = inject(DestroyRef);

  // Signals de estado (inicializan desde sessionStorage si existe)
  readonly #restauranteId = signal<number | null>(this.leerNumeroDeStorage('restauranteId'));
  readonly #comandaId = signal<number | null>(this.leerNumeroDeStorage('comandaId'));
  readonly #mesaId = signal<number | null>(this.leerNumeroDeStorage('mesaId'));
  readonly #mesaInfo = signal<Mesa | null>(null);
  readonly #estadoPedido = signal<EstadoPedido | null>(null);
  readonly #cargando = signal(false);
  readonly #error = signal<string | null>(null);

  // Readonly signals
  restauranteId = this.#restauranteId.asReadonly();
  comandaId = this.#comandaId.asReadonly();
  mesaId = this.#mesaId.asReadonly();
  mesaInfo = this.#mesaInfo.asReadonly();
  estadoPedido = this.#estadoPedido.asReadonly();
  cargando = this.#cargando.asReadonly();
  error = this.#error.asReadonly();

  // Computed
  tieneComandaActiva = computed(() => this.#comandaId() !== null);

  constructor() {
    effect(() => {
      const modificada = this.comandaHub.comandaModificada();
      if (modificada) {
        this.consultarEstado();
      }
    });
  }

  async iniciarEscucha(mesaId: number): Promise<void> {
    await this.comandaHub.conectarComoComensal(mesaId);
  }

  detenerEscucha(): void {
    this.comandaHub.desconectarEscucha();
  }

  /**
   * Ocupar mesa y crear comanda
   * Devuelve Observable para que el componente pueda reaccionar cuando termina
   */
 ocuparMesa(
  restauranteId: number,
  mesaId: number,
  cantidadComensales: number
): Observable<any> {
  this.#restauranteId.set(restauranteId);
  this.#mesaId.set(mesaId);
  this.#cargando.set(true);
  this.#error.set(null);

  return this.comandaService.ocuparMesa(
    restauranteId,
    mesaId,
    cantidadComensales
  ).pipe(
    tap(response => {
      this.#mesaInfo.set(response.mesa);
      this.#comandaId.set(response.idComandaGenerada);

      sessionStorage.setItem('restauranteId', String(restauranteId));
      sessionStorage.setItem('comandaId', String(response.idComandaGenerada));
      sessionStorage.setItem('mesaId', String(mesaId));

      this.#cargando.set(false);
    }),
    catchError(err => {
      console.error('Error al ocupar mesa:', err);
      this.#error.set('No se pudo ocupar la mesa. Intenta nuevamente.');
      this.#cargando.set(false);
      return throwError(() => err);
    })
  );
}



  /**
   * Confirmar pedido (envía items al backend)
   */
  confirmarPedido(): void {

    
    const comandaId = this.#comandaId();
    const restauranteId = this.#restauranteId();

    if (!comandaId || !restauranteId) {
      this.#error.set('No hay comanda activa. Escanea el QR de la mesa.');
      return;
    }

    const pedidos = this.pedidoService.obtenerPedidos();

    if (pedidos.length === 0) {
      this.#error.set('El carrito está vacío');
      return;
    }

    console.log('comandaId:', comandaId);
console.log('restauranteId:', restauranteId);
console.log('pedidos:', pedidos);


    this.#cargando.set(true);
    this.#error.set(null);

    const nombreComensal = sessionStorage.getItem('nombreComensal') ?? '';

    const items = pedidos.map(p => ({
      articuloId: p.plato.articuloId,
      cantidad: p.cantidad,
      observacionesIngredientes: p.observacionesIngredientes ?? null,
      observacionesGenerales: p.observacionesGenerales ?? null
    }));

    this.comandaService.confirmarPedido(comandaId, restauranteId,  {
    items,
    nombreComensal
  } )
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: (response) => {
          const estado: EstadoPedido = {
            comandaId: response.comandaId,
            estadoUI: response.estadoUI,
            totalAPagar: response.totalAPagar,
            items: response.items
          };
          this.#estadoPedido.set(estado);
          this.#cargando.set(false);
          this.pedidoService.limpiarPedidos();
        },
        error: (err) => {
  console.error('Error completo:', err);
  console.log('Error body:', err.error);
  console.log('Errores de validación:', err.error?.errors);

  this.#error.set('Error al confirmar el pedido. Intenta nuevamente.');
  this.#cargando.set(false);
}
      });
  }

  /**
   * Consultar estado del pedido
   */
  consultarEstado(): void {
    const comandaId = this.#comandaId();
    const restauranteId = this.#restauranteId();

    if (!comandaId || !restauranteId) return;

    this.comandaService.obtenerEstado(comandaId, restauranteId)
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: (response) => {
          const estado: EstadoPedido = {
            comandaId: response.comandaId,
            estadoUI: response.estadoUI,
            totalAPagar: response.totalAPagar,
            items: response.items
          };
          this.#estadoPedido.set(estado);
        },
        error: (err) => {
          console.error('Error al consultar estado:', err);
        }
      });
  }

  /**
   * Limpiar estado (para nueva comanda)
   */
  limpiarEstado(): void {
    this.#restauranteId.set(null);
    this.#comandaId.set(null);
    this.#mesaId.set(null);
    this.#mesaInfo.set(null);
    this.#estadoPedido.set(null);
    this.#error.set(null);
    sessionStorage.removeItem('restauranteId');
    sessionStorage.removeItem('comandaId');
    sessionStorage.removeItem('mesaId');
  }

  limpiarError(): void {
    this.#error.set(null);
  }

  private leerNumeroDeStorage(key: string): number | null {
    const val = sessionStorage.getItem(key);
    if (val === null) return null;
    const num = Number(val);
    return Number.isFinite(num) ? num : null;
  }

  obtenerBienvenidaInvitado(comandaId: number) {
  return this.http.get<any>(
    `https://localhost:7204/comanda/${comandaId}/comensal/bienvenida-invitado`
  );
}

}
