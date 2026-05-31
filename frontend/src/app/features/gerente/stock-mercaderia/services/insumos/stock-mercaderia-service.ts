import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../../core/services/api-service';
import { map, Observable } from 'rxjs';
import { Insumo, InsumoResponseDto } from '../../../../../core/models/insumos/insumo';
import { CrearInsumoRequest } from '../../../../../core/models/insumos/crear-insumo-request';


@Injectable({ providedIn: 'root' })

export class StockMercaderiaService {
  private api = inject(ApiService);
  private endpoint = 'Insumo';

  getStockMercaderia(): Observable<Insumo[]> {
    return this.api.get<InsumoResponseDto[]>(this.endpoint).pipe(
      map(dtos => dtos.map(dto => this.toDomain(dto)))
    );
  }

  getById(id: number): Observable<Insumo> {
    return this.api.get<InsumoResponseDto>(`${this.endpoint}/${id}`).pipe(
      map(dto => this.toDomain(dto))
    );
  }

  private toDomain(dto: InsumoResponseDto): Insumo {
    return {
      id: dto.id,
      nombre: dto.nombre,
      stockActual: dto.stockActual,
      stockMinimo: dto.stockMinimo,
      vencimiento: dto.vencimiento ?? '',
      unidadMedida: { id: 0, nombre: dto.unidadMedida }, 
      categoriaIngrediente: { 
        id: 0, 
        descripcion: dto.categoria, 
        tipoAplica: dto.tipo 
      }
    };
  }

 
  crear(producto: CrearInsumoRequest): Observable<Insumo> {
    return this.api.post<InsumoResponseDto>(this.endpoint, producto).pipe(
      map(dto => this.toDomain(dto))
    );
  }

  actualizar(id: number, producto: Partial<Insumo>): Observable<Insumo> {
    return this.api.put<Insumo>(`${this.endpoint}/${id}`, producto);
  }

  eliminar(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
