import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { Proveedor, PedidoProveedor, NuevoPedidoProveedor } from '../../../../core/models/proveedor';
import { Insumo as ProductoStockMock } from '../../../../core/models/producto-stock';

interface ProveedorResponseDto {
  id: number;
  nombre: string | null;
  numeroTelefonoWsp: string | null;
  fechaUltimoPedido: string | null;
  categorias: string[] | null;
}

interface PedidoInsumoResponseDto {
  insumoId: number;
  nombreInsumo: string | null;
  cantidad: number;
  precioCompra: number;
}

interface PedidoResponseDto {
  id: number;
  fecha: string | null;
  estado: string | null;
  itemsInsumo: PedidoInsumoResponseDto[] | null;
}

interface CrearPedidoResponseDto extends PedidoResponseDto {
  proveedorId: number;
  proveedorNombre: string | null;
  proveedorTelefono: string | null;
}

interface CrearPedidoRequestDto {
  items: {
    insumoId: number;
    cantidad: number;
    precioCompra: number;
  }[];
}

interface InsumoResponseDto {
  id: number;
  nombre: string | null;
  stockActual: number;
  unidadMedida: string | null;
  vencimiento: string | null;
  stockMinimo: number;
  estadoStock: string | null;
  tipo: string | null;
  categoria: string | null;
}

@Injectable({ providedIn: 'root' })
export class VerProveedoresApiService {
  private api = inject(ApiClient);

  getProveedores(): Observable<Proveedor[]> {
    return this.api.get<ProveedorResponseDto[]>('Proveedor').pipe(
      map(proveedores => proveedores.map(proveedor => ({
        id: proveedor.id,
        nombre: proveedor.nombre ?? '',
        contacto: proveedor.nombre ?? '',
        telefono: proveedor.numeroTelefonoWsp ?? '',
        email: '',
        direccion: '',
        activo: true,
        fechaUltimoPedido: this.normalizarFecha(proveedor.fechaUltimoPedido),
        categorias: proveedor.categorias ?? []
      })))
    );
  }

  getHistorialPedidos(id: number | string): Observable<PedidoProveedor[]> {
    return this.api.get<PedidoResponseDto[]>(`Proveedor/${id}/historial-pedidos`).pipe(
      map(pedidos => pedidos.map(pedido => this.mapPedido(pedido)))
    );
  }

  getProductosDisponibles(): Observable<ProductoStockMock[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo').pipe(
      map(insumos => insumos.map(insumo => this.mapInsumo(insumo)))
    );
  }

  getInsumosProveedor(id: number | string): Observable<ProductoStockMock[]> {
    return this.api.get<InsumoResponseDto[]>(`Proveedor/${id}/insumos`).pipe(
      map(insumos => insumos.map(insumo => this.mapInsumo(insumo)))
    );
  }

  crearPedidoProveedor(id: number | string, pedido: NuevoPedidoProveedor): Observable<PedidoProveedor> {
    return this.api.post<CrearPedidoResponseDto>(`Proveedor/${id}/crearPedido`, this.mapCrearPedidoRequest(pedido)).pipe(
      map(response => this.mapPedido(response))
    );
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

  private mapPedido(pedido: PedidoResponseDto): PedidoProveedor {
    const items = pedido.itemsInsumo ?? [];
    return {
      id: pedido.id,
      fecha: this.normalizarFecha(pedido.fecha) ?? '',
      concepto: `Pedido #${pedido.id}`,
      monto: items.reduce((total, item) => total + item.cantidad * item.precioCompra, 0),
      estado: this.mapEstado(pedido.estado),
      observacion: '',
      items: items.map(item => ({
        id: item.insumoId,
        nombre: item.nombreInsumo ?? '',
        cantidad: item.cantidad,
        unidadMedida: 'UN',
        precioUnitario: item.precioCompra
      }))
    };
  }

  private mapInsumo(insumo: InsumoResponseDto): ProductoStockMock {
    return {
      id: insumo.id,
      nombre: insumo.nombre ?? '',
      stock: insumo.stockActual,
      fechaVencimiento: insumo.vencimiento ?? '',
      unidadMedida: (insumo.unidadMedida ?? 'UN') as ProductoStockMock['unidadMedida'],
      categoriaIngrediente: (insumo.categoria ?? 'Almacen') as ProductoStockMock['categoriaIngrediente'],
      stockMinimo: insumo.stockMinimo
    };
  }

  private mapEstado(estado: string | null): PedidoProveedor['estado'] {
    if (estado === 'Confirmado' || estado === 'Recibido' || estado === 'Cancelado') return estado;
    return 'Pendiente';
  }

  private normalizarFecha(fecha: string | null): string | null {
    if (!fecha) return null;
    const partes = fecha.split('/');
    if (partes.length !== 3) return fecha;
    const [dia, mes, anio] = partes;
    return `${anio}-${mes.padStart(2, '0')}-${dia.padStart(2, '0')}`;
  }
}
