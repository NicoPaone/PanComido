import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { Proveedor, PedidoProveedor, NuevoPedidoProveedor } from '../../../../core/models/proveedor';
import { Insumo as ProductoStockMock } from '../../../../core/models/producto-stock';

@Injectable({ providedIn: 'root' })
export class VerProveedoresApiService {
  private api = inject(ApiClient);

  /** GET /api/Proveedor — lista todos los proveedores (sin historial embebido) */
  getProveedores(): Observable<Proveedor[]> {
    return this.api.get<Proveedor[]>('Proveedor');
  }

  /** GET /api/Proveedor/{id}/historial-pedidos — historial de pedidos de un proveedor */
  getHistorialPedidos(id: number | string): Observable<PedidoProveedor[]> {
    return this.api.get<PedidoProveedor[]>(`Proveedor/${id}/historial-pedidos`);
  }

  getProductosDisponibles(): Observable<ProductoStockMock[]> {
    return this.api.get<ProductoStockMock[]>('proveedores/productos-disponibles');
  }

  // TODO (HÍBRIDO): En el futuro, el backend procesará el pedido y retornará un DTO
  // con la estructura `{ proveedor: Proveedor, whatsappUrl: string }`.
  // Se deberá actualizar el tipo de retorno aquí para recibir esa URL de WhatsApp ya generada.
  crearPedidoProveedor(id: number | string, pedido: NuevoPedidoProveedor): Observable<Proveedor> {
    return this.api.post<Proveedor>(`Proveedor/${id}/pedidos`, pedido);
  }
}
