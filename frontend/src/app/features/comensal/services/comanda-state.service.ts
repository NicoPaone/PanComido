import { inject, Injectable, signal, computed } from '@angular/core';
import { ComandaService } from './comanda.service';
import { PedidoService } from '../../../core/services/pedido.service';
import { ComandaClienteResponse } from '../../../core/models/comanda-cliente-response';
import { Mesa } from '../../../core/models/mesa.model';
import { ItemPedidoRequest } from '../../../core/models/confirmar-pedido-request';

@Injectable({ providedIn: 'root' })
export class ComandaStateService {
  private comandaService = inject(ComandaService);
  private pedidoService = inject(PedidoService);

  // Signals de estado
  private _comandaId = signal<number | null>(null);
  private _mesaId = signal<number | null>(null);
  private _mesaInfo = signal<Mesa | null>(null);
  private _estadoPedido = signal<ComandaClienteResponse | null>(null);
  private _cargando = signal(false);
  private _error = signal<string | null>(null);

  // Readonly signals
  comandaId = this._comandaId.asReadonly();
  mesaId = this._mesaId.asReadonly();
  mesaInfo = this._mesaInfo.asReadonly();
  estadoPedido = this._estadoPedido.asReadonly();
  cargando = this._cargando.asReadonly();
  error = this._error.asReadonly();

  // Computed
  tieneComandaActiva = computed(() => this._comandaId() !== null);

  /**
   * Ocupar mesa y crear comanda
   * Llamar DESPUÉS de que el usuario ingrese la cantidad de personas
   */
  ocuparMesa(mesaId: number, cantidadComensales: number): Promise<void> {
    this._mesaId.set(mesaId);
    this._cargando.set(true);
    this._error.set(null);

    return new Promise((resolve, reject) => {
      this.comandaService.ocuparMesa(mesaId, cantidadComensales).subscribe({
        next: (response) => {
          this._mesaInfo.set(response.mesa);
          this._comandaId.set(response.idComandaGenerada);
          this._cargando.set(false);
          resolve();
        },
        error: (err) => {
          console.error('Error al ocupar mesa:', err);
          this._error.set('No se pudo ocupar la mesa. Intenta nuevamente.');
          this._cargando.set(false);
          reject(err);
        }
      });
    });
  }

  /**
   * PASO 2: Confirmar pedido (envía items al backend)
   */
  confirmarPedido(): Promise<ComandaClienteResponse> {
    const comandaId = this._comandaId();
    
    if (!comandaId) {
      this._error.set('No hay comanda activa. Escanea el QR de la mesa.');
      return Promise.reject('No hay comanda activa');
    }

    const pedidos = this.pedidoService.obtenerPedidos();
    
    if (pedidos.length === 0) {
      this._error.set('El carrito está vacío');
      return Promise.reject('Carrito vacío');
    }

    this._cargando.set(true);
    this._error.set(null);

    const items: ItemPedidoRequest[] = pedidos.map(p => ({
      articuloId: p.plato.articuloId,
      cantidad: p.cantidad,
      observacionesIngredientes: p.observacionesIngredientes ?? null,
      observacionesGenerales: p.observacionesGenerales ?? null
    }));

    return new Promise((resolve, reject) => {
      this.comandaService.confirmarPedido(comandaId, { items }).subscribe({
        next: (response) => {
          this._estadoPedido.set(response);
          this._cargando.set(false);
          // Limpiamos el carrito después de confirmar
          this.pedidoService.limpiarPedidos();
          resolve(response);
        },
        error: (err) => {
          console.error('Error al confirmar pedido:', err);
          this._error.set('Error al confirmar el pedido. Intenta nuevamente.');
          this._cargando.set(false);
          reject(err);
        }
      });
    });
  }

  /**
   * PASO 3: Consultar estado del pedido
   */
  consultarEstado(): Promise<ComandaClienteResponse> {
    const comandaId = this._comandaId();
    
    if (!comandaId) {
      return Promise.reject('No hay comanda activa');
    }

    return new Promise((resolve, reject) => {
      this.comandaService.obtenerEstado(comandaId).subscribe({
        next: (response) => {
          this._estadoPedido.set(response);
          resolve(response);
        },
        error: (err) => {
          console.error('Error al consultar estado:', err);
          reject(err);
        }
      });
    });
  }

  /**
   * Limpiar estado (para nueva comanda)
   */
  limpiarEstado(): void {
    this._comandaId.set(null);
    this._mesaId.set(null);
    this._mesaInfo.set(null);
    this._estadoPedido.set(null);
    this._error.set(null);
  }

  /**
   * Limpiar solo el error
   */
  limpiarError(): void {
    this._error.set(null);
  }
}
