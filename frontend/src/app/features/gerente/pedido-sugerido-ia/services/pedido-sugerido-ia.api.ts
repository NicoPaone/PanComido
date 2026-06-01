import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';

import { Proveedor, SugerenciaPedidoItem, NuevoPedidoProveedor } from '../../../../core/models/proveedor';
import { Insumo as ProductoStockMock } from '../../../../core/models/producto-stock';
import { ProveedorService } from './proveedor-service';

@Injectable({ providedIn: 'root' })
export class PedidoSugeridoIAApiService {
  
  private api = inject(ApiClient);
  private proveedorService = inject(ProveedorService);

  private isMock(): boolean {
    return !!this.proveedorService && this.proveedorService.constructor.name !== 'ProveedorService';
  }

  getProveedorById(id: number | string): Observable<Proveedor | undefined> {
    if (this.isMock()) {
      return this.proveedorService!.getProveedorById(Number(id));
    }
    return this.api.get<Proveedor>(`proveedores/${id}`);
  }

  getPedidoSugeridoIA(id: number | string): Observable<SugerenciaPedidoItem[]> {
    if (this.isMock()) {
      return this.proveedorService!.getPedidoSugeridoIA(Number(id));
    }
    return this.api.get<SugerenciaPedidoItem[]>(`proveedores/${id}/pedido-sugerido-ia`);
  }

  getProductosDisponibles(): Observable<ProductoStockMock[]> {
    if (this.isMock()) {
      return this.proveedorService!.getProductosDisponibles();
    }
    return this.api.get<ProductoStockMock[]>('proveedores/productos-disponibles');
  }

  crearPedidoProveedor(id: number | string, pedido: NuevoPedidoProveedor): Observable<Proveedor> {
    if (this.isMock()) {
      return this.proveedorService!.crearPedidoProveedor(id, pedido);
    }
    return this.api.post<Proveedor>(`proveedores/${id}/pedidos`, pedido);
  }
}
