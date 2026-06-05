import { Injectable, inject } from '@angular/core';
import { Observable, map, switchMap, of } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { IngredienteVencimiento, VencimientoProveedor, VencimientoPedidoActivo } from '../../../../core/models/domain/vencimiento';
import { Insumo } from '../../../../core/models/domain/insumo';
import { PedidoProveedor } from '../../../../core/models/domain/proveedor';
import { NuevoPedidoProveedor } from '../../../../core/models/dtos/requests/proveedor.request';

interface ProveedorResponseDto {
  id: number;
  nombre: string | null;
  categorias: string[] | null;
}

interface PedidoResponseDto {
  id: number;
  fecha: string | null;
  estado: string | null;
}

interface InsumoResponseDto {
  id: number;
  nombre: string | null;
  stockActual: number;
  unidadMedida: string | null;
  vencimiento: string | null;
  stockMinimo: number;
  categoria: string | null;
}

interface CrearPedidoRequestDto {
  items: {
    insumoId: number;
    cantidad: number;
    precioCompra: number;
  }[];
}

@Injectable({ providedIn: 'root' })
export class VencimientosApiService {
  private api = inject(ApiService);

  getIngredientesProximosVencer(): Observable<IngredienteVencimiento[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo').pipe(
      map(insumos => insumos
        .filter(insumo => this.estaProximoAVencer(insumo.vencimiento))
        .map(insumo => ({
          id: insumo.id,
          nombre: insumo.nombre ?? '',
          fechaVencimiento: insumo.vencimiento ?? '',
          stockDisponible: insumo.stockActual,
         unidadMedida: { 
            id: 0, 
            nombre: insumo.unidadMedida ?? 'UN' 
          }
        }))
      )
    );
  }

  getProveedoresPorIngrediente(ingredienteId: string | number): Observable<VencimientoProveedor[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo').pipe(
      map(insumos => insumos.find(insumo => insumo.id.toString() === ingredienteId.toString())),
      switchMap(ingrediente => ingrediente?.categoria ? this.getProveedoresPorCategoria(ingrediente.categoria) : of([]))
    );
  }

  getProveedoresPorCategoria(categoria: string): Observable<VencimientoProveedor[]> {
    return this.api.get<ProveedorResponseDto[]>('Proveedor').pipe(
      map(proveedores => proveedores
        .filter(proveedor => (proveedor.categorias ?? []).includes(categoria))
        .map(proveedor => ({ id: proveedor.id, nombre: proveedor.nombre ?? '' }))
      )
    );
  }

  getPedidosActivosPorProveedor(proveedorId: string | number): Observable<VencimientoPedidoActivo[]> {
    return this.api.get<PedidoResponseDto[]>(`Proveedor/${proveedorId}/historial-pedidos`).pipe(
      map(pedidos => pedidos
        .filter(pedido => !pedido.estado || pedido.estado === 'Pendiente')
        .map(pedido => ({
          id: pedido.id.toString(),
          numeroEnvio: `PED-${pedido.id}`,
          fechaCreacion: pedido.fecha ?? ''
        }))
      )
    );
  }

  crearPedidoProveedor(proveedorId: string | number, pedido: NuevoPedidoProveedor): Observable<PedidoProveedor> {
    return this.api.post<PedidoProveedor>(`pedido-proveedor/${proveedorId}/crear-pedido`, this.mapCrearPedidoRequest(pedido));
  }

  private mapCrearPedidoRequest(pedido: NuevoPedidoProveedor): CrearPedidoRequestDto {
    return {
      items: pedido.items.map(item => ({
        insumoId: Number(item.id),
        cantidad: item.cantidad,
        precioCompra: item.precioUnitario ?? 1
      }))
    };
  }

  private estaProximoAVencer(fecha: string | null): boolean {
    if (!fecha) return false;
    const hoy = new Date();
    const vencimiento = this.parseFecha(fecha);
    if (Number.isNaN(vencimiento.getTime())) return false;
    const dias = Math.ceil((vencimiento.getTime() - hoy.getTime()) / 86400000);
    return dias <= 30;
  }

  private parseFecha(fecha: string): Date {
    const partes = fecha.split('/');
    if (partes.length === 3) {
      const [dia, mes, anio] = partes.map(Number);
      return new Date(anio, mes - 1, dia);
    }
    return new Date(`${fecha}T00:00:00`);
  }
}
