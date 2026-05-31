import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';

// Modelos de Dominio
import { Proveedor, PedidoProveedor, NuevoPedidoProveedor } from '../../../../core/models/proveedor';
import { Insumo } from '../../../../core/models/insumos/insumo';
import { CategoriaInsumo } from '../../../../core/models/insumos/categorias/categoria-insumo';
import { UnidadMedida } from '../../../../core/models/unidad-medida';

// DTOs (Lo que escupe la red)
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
      map(proveedores => proveedores.map(dto => this.mapProveedor(dto)))
    );
  }

  getHistorialPedidos(id: number | string): Observable<PedidoProveedor[]> {
    return this.api.get<PedidoResponseDto[]>(`Proveedor/${id}/historial-pedidos`).pipe(
      map(pedidos => pedidos.map(dto => this.mapPedido(dto)))
    );
  }

  getProductosDisponibles(): Observable<Insumo[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo').pipe(
      map(insumos => insumos.map(dto => this.mapInsumo(dto)))
    );
  }

  getInsumosProveedor(id: number | string): Observable<Insumo[]> {
    return this.api.get<InsumoResponseDto[]>(`Proveedor/${id}/insumos`).pipe(
      map(insumos => insumos.map(dto => this.mapInsumo(dto)))
    );
  }

  crearPedidoProveedor(id: number | string, pedido: NuevoPedidoProveedor): Observable<PedidoProveedor> {
    const payload = this.mapCrearPedidoRequest(pedido);
    return this.api.post<CrearPedidoResponseDto>(`Proveedor/${id}/crearPedido`, payload).pipe(
      map(response => this.mapPedido(response))
    );
  }


  private mapProveedor(dto: ProveedorResponseDto): Proveedor {
    return {
      id: dto.id,
      nombre: dto.nombre ?? 'Sin Nombre',
      contacto: dto.nombre ?? 'Sin Contacto',
      telefono: dto.numeroTelefonoWsp ?? '',
      email: '',
      direccion: '', 
      activo: true,
      fechaUltimoPedido: this.normalizarFecha(dto.fechaUltimoPedido),
      categorias: dto.categorias ?? []
    };
  }

  private mapCrearPedidoRequest(pedido: NuevoPedidoProveedor): CrearPedidoRequestDto {
    return {
      items: pedido.items.map(item => ({
        insumoId: Number(item.id),
        cantidad: item.cantidad,
        precioCompra: item.precioUnitario ?? 0 
      }))
    };
  }

 private mapPedido(dto: PedidoResponseDto): PedidoProveedor {
    const items = dto.itemsInsumo ?? [];
    return {
      id: dto.id,
      fecha: this.normalizarFecha(dto.fecha) ?? new Date().toISOString().split('T')[0],
      concepto: `Pedido #${dto.id}`,
      monto: items.reduce((total, item) => total + (item.cantidad * item.precioCompra), 0),
      estado: this.mapEstado(dto.estado),
      observacion: '',
      items: items.map(item => ({
        id: item.insumoId,
        nombre: item.nombreInsumo ?? 'Insumo Desconocido',
        cantidad: item.cantidad,
        
        unidadMedida: { 
          id: 0, 
          nombre: 'UN' 
        } as UnidadMedida, 
        
        precioUnitario: item.precioCompra
      }))
    };
  }
  private mapInsumo(dto: InsumoResponseDto): Insumo {
   
    return {
      id: dto.id,
      nombre: dto.nombre ?? 'Sin Nombre',
      stock: dto.stockActual,
      fechaVencimiento: dto.vencimiento ?? '',
      stockMinimo: dto.stockMinimo,
      
      unidadMedida: { 
        id: 0, 
        nombre: dto.unidadMedida ?? 'UN' 
      } as UnidadMedida,
      
      categoriaIngrediente: { 
        id: 0, 
        descripcion: dto.categoria ?? 'Sin Categoría', 
        tipoAplica: dto.tipo ?? 'Ingrediente' 
      } as CategoriaInsumo
    };
  }

  private mapEstado(estado: string | null): PedidoProveedor['estado'] {
    const estadosValidos = ['Confirmado', 'Recibido', 'Cancelado'];
    return estadosValidos.includes(estado as string) ? (estado as PedidoProveedor['estado']) : 'Pendiente';
  }


  private normalizarFecha(fecha: string | null): string | null {
    if (!fecha) return null;
    
    if (fecha.includes('-')) {
        return fecha.split('T')[0]; 
    }

    const partes = fecha.split('/');
    if (partes.length !== 3) return fecha;
    const [dia, mes, anio] = partes;
    return `${anio}-${mes.padStart(2, '0')}-${dia.padStart(2, '0')}`;
  }
}