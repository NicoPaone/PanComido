import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { Proveedor, PedidoProveedor, NuevoPedidoProveedor, PedidoProveedorItem } from '../../../../core/models/proveedor';
import { Insumo as ProductoStockMock } from '../../../../core/models/producto-stock';

@Injectable({ providedIn: 'root' })
export class VerProveedoresApiService {
  private api = inject(ApiClient);

  getProveedores(): Observable<Proveedor[]> {
    return this.api.get<Proveedor[]>('Proveedor');
  }

  getHistorialPedidos(id: number | string): Observable<PedidoProveedor[]> {
    return this.api.get<PedidoProveedor[]>(`Proveedor/${id}/historial-pedidos`);
  }

  getProductosDisponibles(): Observable<ProductoStockMock[]> {
    return this.api.get<ProductoStockMock[]>('proveedores/productos-disponibles');
  }

  crearPedidoProveedor(id: number | string, pedido: NuevoPedidoProveedor): Observable<Proveedor> {
    return this.api.post<Proveedor>(`Proveedor/${id}/pedidos`, pedido);
  }

  confirmarPedidoProveedor(proveedorId: number | string, pedidoId: number | string): Observable<PedidoProveedor[]> {
    return this.api.post<PedidoProveedor[]>(`Proveedor/${proveedorId}/pedidos/${pedidoId}/confirmar`, {});
  }

  agregarItemPedidoProveedor(proveedorId: number | string, pedidoId: number | string, item: PedidoProveedorItem): Observable<PedidoProveedor[]> {
    return this.api.post<PedidoProveedor[]>(`Proveedor/${proveedorId}/pedidos/${pedidoId}/items`, item);
  }
}
