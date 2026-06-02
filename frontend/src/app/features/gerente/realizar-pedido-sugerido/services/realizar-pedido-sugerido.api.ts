import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';

import { Proveedor, SugerenciaPedidoItem, NuevoPedidoProveedor } from '../../../../core/models/proveedor';
import { Insumo } from '../../../../core/models/insumos/insumo';
import { UnidadMedida } from '../../../../core/models/unidad-medida';
import { CategoriaInsumo } from '../../../../core/models/insumos/categorias/categoria-insumo';

@Injectable({ providedIn: 'root' })
export class RealizarPedidoSugeridoApiService {
  
  private api = inject(ApiClient);

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

  crearPedidoProveedor(id: number | string, pedido: NuevoPedidoProveedor): Observable<unknown> {
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
}
