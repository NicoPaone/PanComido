import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { ProveedorService } from '../../../../core/services/proveedor.service';
import { Proveedor, SugerenciaPedidoItem, NuevoPedidoProveedor } from '../../../../core/models/proveedor';
import { ProductoStockMock } from '../../../../core/model/producto-stock-mock';

@Injectable({ providedIn: 'root' })
export class PedidoSugeridoIAApiService {
  private api = inject(ApiClient);
  private proveedorService = inject(ProveedorService, { optional: true });

  private isMock(): boolean {
    return !!this.proveedorService && this.proveedorService.constructor.name !== 'ProveedorService';
  }

  getProveedorById(id: number): Observable<Proveedor | undefined> {
    if (this.isMock()) {
      return this.proveedorService!.getProveedorById(id);
    }
    return this.api.get<Proveedor>(`proveedores/${id}`);
  }

  getPedidoSugeridoIA(id: number): Observable<SugerenciaPedidoItem[]> {
    if (this.isMock()) {
      return this.proveedorService!.getPedidoSugeridoIA(id);
    }
    return this.api.get<SugerenciaPedidoItem[]>(`proveedores/${id}/pedido-sugerido-ia`);
  }

  getProductosDisponibles(): Observable<ProductoStockMock[]> {
    if (this.isMock()) {
      return this.proveedorService!.getProductosDisponibles();
    }
    return this.api.get<ProductoStockMock[]>('proveedores/productos-disponibles');
  }

  crearPedidoProveedor(id: number, pedido: NuevoPedidoProveedor): Observable<Proveedor> {
    if (this.isMock()) {
      return this.proveedorService!.crearPedidoProveedor(id, pedido);
    }
    return this.api.post<Proveedor>(`proveedores/${id}/pedidos`, pedido);
  }
}
