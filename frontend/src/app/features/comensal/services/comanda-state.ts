import { inject, Injectable, signal, computed, effect } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ComandaService } from './comanda.service';
import { PedidoState } from './pedido.state';
import { EstadoPedido } from '../../../core/models/domain/comanda';
import { Mesa } from '../../../core/models/domain/mesa';
import { ComandaHubService } from '../../../core/services/hubs/comanda/comanda-hub-service';

@Injectable({ providedIn: 'root' })
export class ComandaState {
  private comandaService = inject(ComandaService);
  private pedidoService = inject(PedidoState);
  private comandaHub = inject(ComandaHubService);

  // Signals de estado (inicializan desde sessionStorage si existe)
  readonly #comandaId = signal<number | null>(this.leerNumeroDeStorage('comandaId'));
  readonly #mesaId = signal<number | null>(this.leerNumeroDeStorage('mesaId'));
  readonly #mesaInfo = signal<Mesa | null>(null);
  readonly #estadoPedido = signal<EstadoPedido | null>(null);
  readonly #cargando = signal(false);
  readonly #error = signal<string | null>(null);

  // Readonly signals
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
   * Llamar DESPUÉS de que el usuario ingrese la cantidad de personas
   */
  async ocuparMesa(mesaId: number, cantidadComensales: number): Promise<void> {
    this.#mesaId.set(mesaId);
    this.#cargando.set(true);
    this.#error.set(null);

    try {
      const response = await firstValueFrom(this.comandaService.ocuparMesa(mesaId, cantidadComensales));
      this.#mesaInfo.set(response.mesa);
      this.#comandaId.set(response.idComandaGenerada);
      sessionStorage.setItem('comandaId', String(response.idComandaGenerada));
      sessionStorage.setItem('mesaId', String(mesaId));
      this.#cargando.set(false);
    } catch (err: any) {
      console.error('Error al ocupar mesa:', err);
      this.#error.set('No se pudo ocupar la mesa. Intenta nuevamente.');
      this.#cargando.set(false);
      throw err;
    }
  }

  /**
   * PASO 2: Confirmar pedido (envía items al backend)
   */
  async confirmarPedido(): Promise<EstadoPedido> {
    const comandaId = this.#comandaId();
    
    if (!comandaId) {
      this.#error.set('No hay comanda activa. Escanea el QR de la mesa.');
      return Promise.reject('No hay comanda activa');
    }

    const pedidos = this.pedidoService.obtenerPedidos();
    
    if (pedidos.length === 0) {
      this.#error.set('El carrito está vacío');
      return Promise.reject('Carrito vacío');
    }

    this.#cargando.set(true);
    this.#error.set(null);

    const items = pedidos.map(p => ({
      articuloId: p.plato.articuloId,
      cantidad: p.cantidad,
      observacionesIngredientes: p.observacionesIngredientes ?? null,
      observacionesGenerales: p.observacionesGenerales ?? null
    }));

    try {
      const response = await firstValueFrom(this.comandaService.confirmarPedido(comandaId, { items }));
      const estado: EstadoPedido = {
        comandaId: response.comandaId,
        estadoUI: response.estadoUI,
        totalAPagar: response.totalAPagar,
        items: response.items
      };
      this.#estadoPedido.set(estado);
      this.#cargando.set(false);
      this.pedidoService.limpiarPedidos();
      return estado;
    } catch (err: any) {
      console.error('Error al confirmar pedido:', err);
      this.#error.set('Error al confirmar el pedido. Intenta nuevamente.');
      this.#cargando.set(false);
      throw err;
    }
  }

  /**
   * PASO 3: Consultar estado del pedido
   */
  async consultarEstado(): Promise<EstadoPedido> {
    const comandaId = this.#comandaId();
    
    if (!comandaId) {
      return Promise.reject('No hay comanda activa');
    }

    try {
      const response = await firstValueFrom(this.comandaService.obtenerEstado(comandaId));
      const estado: EstadoPedido = {
        comandaId: response.comandaId,
        estadoUI: response.estadoUI,
        totalAPagar: response.totalAPagar,
        items: response.items
      };
      this.#estadoPedido.set(estado);
      return estado;
    } catch (err: any) {
      console.error('Error al consultar estado:', err);
      throw err;
    }
  }

  /**
   * Limpiar estado (para nueva comanda)
   */
  limpiarEstado(): void {
    this.#comandaId.set(null);
    this.#mesaId.set(null);
    this.#mesaInfo.set(null);
    this.#estadoPedido.set(null);
    this.#error.set(null);
    sessionStorage.removeItem('comandaId');
    sessionStorage.removeItem('mesaId');
  }

  /**
   * Limpiar solo el error
   */
  limpiarError(): void {
    this.#error.set(null);
  }
  
  private leerNumeroDeStorage(key: string): number | null {
    const val = sessionStorage.getItem(key);
    if (val === null) return null;
    const num = Number(val);
    return Number.isFinite(num) ? num : null;
  }
}
