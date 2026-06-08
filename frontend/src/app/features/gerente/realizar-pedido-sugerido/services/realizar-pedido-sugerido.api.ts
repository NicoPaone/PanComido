import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';

import { Proveedor, SugerenciaPedidoItem, PedidoProveedorRequest } from '../../../../core/models/domain/proveedor';
import { Insumo } from '../../../../core/models/domain/insumo';
import { UnidadMedida } from '../../../../core/models/domain/unidad-medida';
import { CategoriaInsumo } from '../../../../core/models/domain/categoria-insumo';

@Injectable({ providedIn: 'root' })
export class RealizarPedidoSugeridoApiService {

  private api = inject(ApiService);

  getProveedores(): Observable<Proveedor[]> {
    return this.api.get<any[]>('Proveedor').pipe(
      map(proveedores => proveedores.map(proveedor => this.mapProveedor(proveedor)))
    );
  }

  getProveedorById(id: number | string): Observable<Proveedor | undefined> {
    return this.api.get<any[]>('Proveedor').pipe(
      map(proveedores => proveedores
        .map(proveedor => this.mapProveedor(proveedor))
        .find(proveedor => proveedor.id.toString() === id.toString()))
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

  getProductosDisponibles(): Observable<Insumo[]> {
    return this.api.get<any[]>('Insumo').pipe(
      map(insumos => insumos.map(insumo => ({
        id: insumo.id,
        nombre: insumo.nombre ?? '',
        stockActual: insumo.stockActual ?? insumo.stock ?? 0,
        vencimiento: insumo.vencimiento ?? insumo.fechaVencimiento ?? '',
        unidadMedida: this.mapUnidadMedida(insumo.unidadMedida),
        categoriaIngrediente: this.mapCategoriaInsumo(insumo.categoria ?? insumo.categoriaIngrediente),
        stockMinimo: insumo.stockMinimo ?? 0
      })))
    );
  }

  getInsumosProveedor(id: number | string): Observable<Insumo[]> {
    return this.api.get<any[]>(`Proveedor/${id}/insumos`).pipe(
      map(insumos => insumos.map(insumo => ({
        id: insumo.id,
        nombre: insumo.nombre ?? '',
        stockActual: insumo.stockActual ?? insumo.stock ?? 0,
        vencimiento: insumo.vencimiento ?? insumo.fechaVencimiento ?? '',
        unidadMedida: this.mapUnidadMedida(insumo.unidadMedida),
        categoriaIngrediente: this.mapCategoriaInsumo(insumo.categoria ?? insumo.categoriaIngrediente),
        stockMinimo: insumo.stockMinimo ?? 0
      })))
    );
  }

  crearPedidoProveedor(id: number | string, pedido: PedidoProveedorRequest): Observable<unknown> {
    return this.api.post<unknown>(`pedido-proveedor/${id}/crear-pedido`, {
      items: pedido.items.map(item => ({
        insumoId: Number(item.id),
        cantidad: item.cantidad,
        precioCompra: this.normalizarPrecio(item.precioUnitario)
      }))
    });
  }

  private mapProveedor(proveedor: any): Proveedor {
    return {
      id: proveedor.id,
      nombre: proveedor.nombre ?? '',
      contacto: proveedor.nombre ?? '',
      telefono: proveedor.numeroTelefonoWsp ?? proveedor.numeroTelefonoWSP ?? proveedor.telefono ?? '',
      email: proveedor.email ?? proveedor.correo ?? '',
      direccion: proveedor.direccion ?? '',
      activo: proveedor.activo ?? true,
      fechaUltimoPedido: proveedor.fechaUltimoPedido ?? null,
      categorias: proveedor.categorias ?? []
    };
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

  private mapCategoriaInsumo(valor: unknown): CategoriaInsumo {
    if (typeof valor === 'object' && valor !== null && 'descripcion' in valor) {
      return valor as CategoriaInsumo;
    }
    return { id: 0, descripcion: String(valor ?? 'Almacen'), tipoAplica: 'Ingrediente' };
  }

  getHistorialPedidos(id: number | string): Observable<{ items: { id: string | number; cantidad: number; precioUnitario: number }[]; fecha: string }[]> {
    return this.api.get<any[]>(`Proveedor/${id}/historial-pedidos`).pipe(
      map(pedidos => pedidos.map(p => ({
        fecha: p.fecha ?? '',
        items: (p.itemsInsumo ?? []).map((item: any) => ({
          id: item.insumoId,
          cantidad: item.cantidad ?? 0,
          precioUnitario: item.precioCompra ?? 0
        }))
      })))
    );
  }
}
