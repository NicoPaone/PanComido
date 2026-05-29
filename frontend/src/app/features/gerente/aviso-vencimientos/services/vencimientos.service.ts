import { Injectable, inject } from '@angular/core';
import { Observable, map, switchMap, of } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { IngredienteVencimiento, VencimientoProveedor, VencimientoPedidoActivo } from '../../../../core/models/vencimientos.model';
import { Insumo } from '../../../../core/models/producto-stock';
import { NuevoPedidoProveedor, PedidoProveedor, Proveedor } from '../../../../core/models/proveedor';

@Injectable({ providedIn: 'root' })
export class VencimientosApiService {
  private api = inject(ApiClient);

  getIngredientesProximosVencer(): Observable<IngredienteVencimiento[]> {
    return this.api.get<Insumo[]>('proveedores/productos-disponibles').pipe(
      map(insumos => insumos
        .filter(insumo => this.estaProximoAVencer(insumo.fechaVencimiento))
        .map(insumo => ({
          id: insumo.id,
          nombre: insumo.nombre,
          fechaVencimiento: insumo.fechaVencimiento,
          stockDisponible: insumo.stock,
          unidadMedida: insumo.unidadMedida
        }))
      )
    );
  }

  getProveedoresPorIngrediente(ingredienteId: string | number): Observable<VencimientoProveedor[]> {
    return this.api.get<Insumo[]>('proveedores/productos-disponibles').pipe(
      map(insumos => insumos.find(insumo => insumo.id.toString() === ingredienteId.toString())),
      switchMap(ingrediente => ingrediente ? this.getProveedoresPorCategoria(ingrediente.categoriaIngrediente) : of([]))
    );
  }

  getProveedoresPorCategoria(categoria: string): Observable<VencimientoProveedor[]> {
    return this.api.get<Proveedor[]>('Proveedor').pipe(
      map(proveedores => proveedores
        .filter(proveedor => proveedor.activo && (proveedor.categorias ?? []).includes(categoria))
        .map(proveedor => ({ id: proveedor.id, nombre: proveedor.nombre }))
      )
    );
  }

  getPedidosActivosPorProveedor(proveedorId: string | number): Observable<VencimientoPedidoActivo[]> {
    return this.api.get<PedidoProveedor[]>(`Proveedor/${proveedorId}/historial-pedidos`).pipe(
      map(pedidos => pedidos
        .filter(pedido => pedido.estado === 'Pendiente')
        .map(pedido => ({
          id: pedido.id.toString(),
          numeroEnvio: `PED-${pedido.id}`,
          fechaCreacion: pedido.fecha
        }))
      )
    );
  }

  crearPedidoProveedor(proveedorId: string | number, pedido: NuevoPedidoProveedor): Observable<Proveedor> {
    return this.api.post<Proveedor>(`Proveedor/${proveedorId}/pedidos`, pedido);
  }

  private estaProximoAVencer(fecha: string): boolean {
    const hoy = new Date();
    const vencimiento = new Date(`${fecha}T00:00:00`);
    const dias = Math.ceil((vencimiento.getTime() - hoy.getTime()) / 86400000);
    return dias <= 30;
  }
}
