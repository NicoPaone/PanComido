import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../../core/services/api-service';
import { map, Observable } from 'rxjs';
import { Insumo } from '../../../../../core/models/domain/insumo';
import { InsumoResponseDto } from '../../../../../core/models/dtos/responses/insumo.response';
import { CrearInsumoRequest } from '../../../../../core/models/dtos/requests/crear-insumo.request';


import { mapInsumoDtoToDomain } from '../../../../../infra/http/mappers/insumo.mapper';

@Injectable({ providedIn: 'root' })

export class StockMercaderiaService {
  private api = inject(ApiService);
  private endpoint = 'insumo';

  getStockMercaderia(): Observable<Insumo[]> {
    return this.api.get<InsumoResponseDto[]>(this.endpoint).pipe(
      map(dtos => dtos.map(mapInsumoDtoToDomain))
    );
  }

  getById(id: number): Observable<Insumo> {
    return this.api.get<InsumoResponseDto>(`${this.endpoint}/${id}`).pipe(
      map(mapInsumoDtoToDomain)
    );
  }

  crear(producto: CrearInsumoRequest): Observable<Insumo> {
    return this.api.post< {insumo: InsumoResponseDto, mensaje: string} >(this.endpoint, producto).pipe(
      map(response => mapInsumoDtoToDomain(response.insumo))
    );
  }

  actualizar(id: number, producto: Partial<Insumo>): Observable<Insumo> {
    return this.api.put<Insumo>(`${this.endpoint}/${id}`, producto);
  }

  eliminar(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
