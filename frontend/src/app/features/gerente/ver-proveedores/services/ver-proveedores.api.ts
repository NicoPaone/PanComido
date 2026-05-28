import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { Proveedor, NuevoPedidoProveedor } from '../../../../core/models/proveedor';
import { ProductoStockMock } from '../../../../core/model/producto-stock-mock';

@Injectable({ providedIn: 'root' })
export class VerProveedoresApiService {
  private api = inject(ApiClient);

  getProveedores(): Observable<Proveedor[]> {
    return this.api.get<Proveedor[]>('proveedores');
  }

  getProductosDisponibles(): Observable<ProductoStockMock[]> {
    return this.api.get<ProductoStockMock[]>('proveedores/productos-disponibles');
  }

  crearPedidoProveedor(id: number, pedido: NuevoPedidoProveedor): Observable<Proveedor> {
    return this.api.post<Proveedor>(`proveedores/${id}/pedidos`, pedido);
  }
}
