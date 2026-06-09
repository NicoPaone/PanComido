import { Injectable, inject } from '@angular/core';
import { catchError, forkJoin, map, Observable, of } from 'rxjs';
import { ApiService } from '../../../core/services/api-service';
import { AuthService } from '../../../core/services/auth.service';

// Modelos de Dominio
import { Proveedor, PedidoProveedor, PedidoProveedorRequest, ProveedorNuevo, RecepcionPedidoItem, SugerenciaPedidoItem } from '../../../core/models/domain/proveedor';
import { Insumo } from '../../../core/models/domain/insumo';
import { CategoriaInsumo } from '../../../core/models/domain/categoria-insumo';
import { UnidadMedida } from '../../../core/models/domain/unidad-medida';
import { Bodega } from '../../../core/models/domain/bodega';
import { InsumoResponseDto } from '../../../core/models/dtos/responses/insumo.response';
import {
  ConfirmarPedidoResponseDto,
  CrearPedidoResponseDto,
  PedidoResponseDto,
  PreRecepcionItemResponseDto,
  ProveedorResponseDto,
} from '../../../core/models/dtos/responses/proveedor.response';
import { CrearPedidoProveedorRequestDto } from '../../../core/models/dtos/requests/crear-pedido-proveedor.request';

interface ProveedorRequestDto {
  nombre: string;
  numeroTelefonoWsp?: string | null;
  categoriaIds: number[];
}

interface ProveedorMutationResponseDto {
  proveedorDto: ProveedorResponseDto;
  mensaje: string;
}

@Injectable({ providedIn: 'root' })
export class ProveedorApiService {
  private api = inject(ApiService);
  private authService = inject(AuthService);

  validateManagerCredentials(user: string, pass: string): Observable<boolean> {
    return this.authService.validateManagerCredentials(user, pass);
  }

  getProveedores(): Observable<Proveedor[]> {
    return this.api.get<ProveedorResponseDto[]>('Proveedor').pipe(
      map(proveedores => proveedores.map(dto => this.mapProveedor(dto)))
    );
  }

  getProveedorById(id: number | string): Observable<Proveedor | undefined> {
    return this.getProveedores().pipe(
      map(proveedores => proveedores.find(proveedor => proveedor.id.toString() === id.toString()))
    );
  }

  crearProveedor(proveedor: ProveedorNuevo): Observable<Proveedor> {
    return this.api.post<ProveedorMutationResponseDto>('proveedor/crear-proveedor', this.mapProveedorRequest(proveedor)).pipe(
      map(response => this.mapProveedor(response.proveedorDto))
    );
  }

  modificarProveedor(id: number | string, proveedor: ProveedorNuevo): Observable<Proveedor> {
    return this.api.patch<ProveedorMutationResponseDto>(`proveedor/${id}/modificar-proveedor`, this.mapProveedorRequest(proveedor)).pipe(
      map(response => this.mapProveedor(response.proveedorDto))
    );
  }

  eliminarProveedor(id: number | string): Observable<unknown> {
    return this.api.delete<unknown>(`proveedor/${id}`);
  }

  getCategoriasInsumo(): Observable<CategoriaInsumo[]> {
    return this.api.get<CategoriaInsumo[]>('categoria-insumo');
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

  getInsumosAReponer(id: number | string): Observable<SugerenciaPedidoItem[]> {
    return this.api.get<any[]>(`Proveedor/${id}/insumos-a-reponer`).pipe(
      map(items => items.map(item => ({
        productoId: item.id.toString(),
        nombre: item.nombre ?? '',
        unidadMedida: this.mapUnidadMedida(item.unidadMedida),
        stockActual: item.stockActual ?? 0,
        stockMinimo: 0,
        estadoStock: item.estadoStock ?? '',
        cantidadSugerida: item.cantidadSugerida ?? 1,
        precioUnitario: this.normalizarPrecio(item.precioUnitario)
      })))
    );
  }

  getInsumosProveedor(id: number | string): Observable<Insumo[]> {
    return forkJoin({
      proveedor: this.api.get<InsumoResponseDto[]>(`Proveedor/${id}/insumos`),
      todos: this.api.get<InsumoResponseDto[]>('Insumo').pipe(
        catchError(() => of([] as InsumoResponseDto[]))
      )
    }).pipe(
      map(({ proveedor, todos }) => {
        const stockPorInsumo = new Map(todos.map(insumo => [insumo.id, insumo]));

        return proveedor.map(dto => {
          const insumoConStock = stockPorInsumo.get(dto.id);
          const stockActual = dto.stockActual || insumoConStock?.stockActual || 0;

          return this.mapInsumo({
            ...dto,
            stockActual,
            vencimiento: dto.vencimiento || insumoConStock?.vencimiento || null,
            estadoStock: dto.estadoStock || insumoConStock?.estadoStock || ''
          });
        });
      })
    );
  }

  crearPedidoProveedor(id: number | string, pedido: PedidoProveedorRequest): Observable<PedidoProveedor> {
    return this.api.post<CrearPedidoResponseDto>(`pedido-proveedor/${id}/crear-pedido`, this.mapCrearPedidoRequest(pedido)).pipe(
      map(response => this.mapPedido(response))
    );
  }

  private mapProveedor(dto: ProveedorResponseDto): Proveedor {
    const telefono = dto.numeroTelefonoWsp ?? dto.numeroTelefonoWSP ?? dto.telefono ?? '';

    return {
      id: dto.id,
      nombre: dto.nombre ?? 'Sin Nombre',
      contacto: dto.nombre ?? 'Sin Contacto',
      telefono,
      email: dto.email ?? dto.correo ?? '',
      direccion: '', 
      activo: true,
      fechaUltimoPedido: this.normalizarFecha(dto.fechaUltimoPedido),
      categorias: dto.categorias ?? []
    };
  }

  private mapProveedorRequest(proveedor: ProveedorNuevo): ProveedorRequestDto {
    return {
      nombre: proveedor.nombre.trim(),
      numeroTelefonoWsp: proveedor.numeroTelefonoWsp?.trim() || null,
      categoriaIds: proveedor.categoriaIds
    };
  }

  confirmarPedido(pedido: PedidoProveedor): Observable<{ pedido: PedidoProveedor; linkWpp: string }> {
    return this.api.put<ConfirmarPedidoResponseDto>(`pedido-proveedor/${pedido.id}/confirmar`, {
      listaInsumosPedido: pedido.items.map(item => ({
        insumoId: Number(item.id),
        cantidad: item.cantidad,
        precioCompra: item.precioUnitario ?? 1
      }))
    }).pipe(
      map(response => ({
        pedido: this.mapPedido(response.pedidoConfirmado),
        linkWpp: response.linkWpp
      }))
    );
  }

  previsualizarConfirmacion(pedidoId: number | string): Observable<RecepcionPedidoItem[]> {
    return this.api.get<PreRecepcionItemResponseDto[]>(`pedido-proveedor/${pedidoId}/previsualizar-confirmacion`).pipe(
      map(items => items.map(item => ({
        insumoId: item.insumoId,
        nombreInsumo: item.nombreInsumo ?? '',
        cantidad: item.cantidad,
        nombreLote: item.nombreLote ?? '',
        bodegaId: item.bodegaIdSug,
        fechaVencimiento: this.normalizarFecha(item.fechaVencimientoSug) ?? ''
      })))
    );
  }

  recibirPedido(pedidoId: number | string, items: RecepcionPedidoItem[]): Observable<unknown> {
    return this.api.put<unknown>(`pedido-proveedor/${pedidoId}/recibir`, {
      itemsPedidoRecibido: items.map(item => ({
        insumoId: item.insumoId,
        nombreLote: item.nombreLote,
        bodegaId: item.bodegaId,
        cantidad: item.cantidad,
        fechaVencimiento: item.fechaVencimiento
      }))
    });
  }

  getBodegas(): Observable<Bodega[]> {
    return this.api.get<Bodega[]>('Bodega');
  }

  getHistorialCantidadPedidos(id: number | string): Observable<{ items: { id: string | number; cantidad: number; precioUnitario: number }[]; fecha: string }[]> {
    return this.api.get<any[]>(`Proveedor/${id}/historial-pedidos`).pipe(
      map(pedidos => pedidos.map(pedido => ({
        fecha: pedido.fecha ?? '',
        items: (pedido.itemsInsumo ?? []).map((item: any) => ({
          id: item.insumoId,
          cantidad: item.cantidad ?? 0,
          precioUnitario: item.precioCompra ?? 0
        }))
      })))
    );
  }

  private mapCrearPedidoRequest(pedido: PedidoProveedorRequest): CrearPedidoProveedorRequestDto {
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
      stockActual: dto.stockActual,
      vencimiento: this.normalizarFecha(dto.vencimiento) ?? '',
      stockMinimo: dto.stockMinimo,
      precioVentaFinal: dto.precioVentaFinal ?? 0,
      
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
    const normalizado = (estado ?? '').toLowerCase();
    if (normalizado === 'enviado') return 'Enviado';
    if (normalizado === 'recibido') return 'Recibido';
    return 'Pendiente';
  }

  private normalizarPrecio(precio: unknown): number {
    const valor = Number(precio);
    return Number.isFinite(valor) && valor >= 1 ? valor : 1;
  }

  private mapUnidadMedida(valor: unknown): UnidadMedida {
    if (typeof valor === 'object' && valor !== null && 'nombre' in valor) {
      return valor as UnidadMedida;
    }
    return { id: 0, nombre: String(valor ?? 'UN') };
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
