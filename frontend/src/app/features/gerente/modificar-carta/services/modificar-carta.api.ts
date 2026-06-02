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
    return this.api.get<Plato[]>('platos');
  }

  getInsumos(): Observable<Insumo[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo').pipe(
      map(insumos => insumos.map(insumo => this.mapInsumo(insumo)))
    );
  }

  updatePlato(id: number, data: Partial<Plato>): Observable<Plato> {
    return this.api.put<Plato>(`/platos/${id}`, data);
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
