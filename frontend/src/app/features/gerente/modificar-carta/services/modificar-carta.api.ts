import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { Plato } from '../../../../core/models/plato';
import { Insumo } from '../../../../core/models/insumos/insumo';
import { UnidadMedida } from '../../../../core/models/unidad-medida';
import { CategoriaInsumo } from '../../../../core/models/insumos/categorias/categoria-insumo';

interface InsumoResponseDto {
  id: number;
  nombre: string | null;
  stockActual: number;
  unidadMedida: string | UnidadMedida | null;
  vencimiento: string | null;
  stockMinimo: number;
  tipo: string | null;
  categoria: string | CategoriaInsumo | null;
}

@Injectable({ providedIn: 'root' })
export class ModificarCartaApiService {
  private api = inject(ApiClient);

  getPlatos(): Observable<Plato[]> {
    return this.api.get<any[]>('carta/obtener-articulos').pipe(
      map(articulos => articulos.map(dto => ({
        id: dto.articuloId,
        nombre: dto.nombre,
        precioVenta: dto.precioVentaFinal,
        costo: dto.costo,
        visible: dto.visibleEnCarta,
        imagen: dto.urlImagen,
        tipo: dto.tipoArticulo,
        categoria: dto.tipoArticulo,
        tiempo: 0,
        bebida: '',
        restriccion: '',
        descripcion: '',
        platoDelDia: false,
        recomendado: false,
        ventas: 0
      })))
    );
  }

  getInsumos(): Observable<Insumo[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo').pipe(
      map(insumos => insumos.map(insumo => this.mapInsumo(insumo)))
    );
  }

  updatePlato(id: number, data: Partial<Plato>): Observable<Plato> {
    // Map frontend specific fields to the ones backend expects
    const payload: any = { ...data };
    if (data.visible !== undefined) {
      payload.visibleEnCarta = data.visible;
      delete payload.visible;
    }
    if (data.precioVenta !== undefined) {
      payload.precioVentaFinal = data.precioVenta;
      delete payload.precioVenta;
    }
    if (data.imagen !== undefined) {
      payload.urlImagen = data.imagen;
      delete payload.imagen;
    }

    // Since the API returns the updated DTO, map it back to Plato if necessary,
    // or return the data so optimistic updates don't break.
    return this.api.patch<any>(`carta/articulos/${id}`, payload).pipe(
      map(dto => ({
        ...data, // Keep optimistic properties
        id: dto.articuloId ?? id,
        nombre: dto.nombre ?? data.nombre,
        precioVenta: dto.precioVentaFinal ?? data.precioVenta,
        costo: dto.costo ?? data.costo,
        visible: dto.visibleEnCarta ?? data.visible,
        imagen: dto.urlImagen ?? data.imagen,
        tipo: dto.tipoArticulo ?? data.tipo,
        categoria: dto.tipoArticulo ?? data.categoria
      } as Plato))
    );
  }

  deletePlato(id: number): Observable<boolean> {
    return this.api.delete<boolean>(`/platos/${id}`);
  }

  private mapInsumo(dto: InsumoResponseDto): Insumo {
    return {
      id: dto.id,
      nombre: dto.nombre ?? '',
      stockActual: dto.stockActual,
      vencimiento: dto.vencimiento ?? '',
      stockMinimo: dto.stockMinimo,
      unidadMedida: this.mapUnidadMedida(dto.unidadMedida),
      categoriaIngrediente: this.mapCategoria(dto.categoria, dto.tipo)
    };
  }

  private mapUnidadMedida(valor: string | UnidadMedida | null): UnidadMedida {
    if (typeof valor === 'object' && valor !== null) return valor;
    return { id: 0, nombre: valor ?? 'UN' };
  }

  private mapCategoria(valor: string | CategoriaInsumo | null, tipo: string | null): CategoriaInsumo {
    if (typeof valor === 'object' && valor !== null) return valor;
    return {
      id: 0,
      descripcion: valor ?? 'Sin categoria',
      tipoAplica: tipo ?? 'Ingrediente'
    };
  }
}
