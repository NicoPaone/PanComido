import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { Plato } from '../../../../core/models/domain/plato';
import { Insumo } from '../../../../core/models/domain/insumo';
import { InsumoResponseDto } from '../../../../core/models/dtos/responses/insumo.response';
import { mapInsumoDtoToDomain } from '../../../../infra/http/mappers/insumo.mapper';
import { environment } from '../../../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class ModificarCartaApiService {
  private api = inject(ApiService);

  getPlatos(): Observable<Plato[]> {
    return this.api.get<any[]>('carta/obtener-articulos').pipe(
      map(articulos => articulos.map(dto => ({
        id: dto.articuloId,
        nombre: dto.nombre,
        precioVenta: dto.precioVentaFinal,
        costo: dto.costo,
        visible: dto.visibleEnCarta,
        imagen: dto.urlImagen ? environment.apiUrl + dto.urlImagen : '',
        tipo: dto.tipoArticulo,
        categoria: dto.categoria
      })))
    );
  }

  getInsumos(): Observable<Insumo[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo').pipe(
      map(insumos => insumos.map(mapInsumoDtoToDomain))
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

    return this.api.patch<any>(`carta/articulos/${id}`, payload).pipe(
      map(dto => {
        const result: Partial<Plato> = { ...data };
        if (dto) {
          if (dto.articuloId !== undefined) result.id = dto.articuloId;
          if (dto.nombre !== undefined) result.nombre = dto.nombre;
          if (dto.precioVentaFinal !== undefined) result.precioVenta = dto.precioVentaFinal;
          if (dto.costo !== undefined) result.costo = dto.costo;
          if (dto.visibleEnCarta !== undefined) result.visible = dto.visibleEnCarta;
          if (dto.urlImagen !== undefined) result.imagen = dto.urlImagen ? environment.apiUrl + dto.urlImagen : '';
          if (dto.tipoArticulo !== undefined) {
            result.tipo = dto.tipoArticulo;
          }
          if (dto.categoria !== undefined) {
            result.categoria = dto.categoria;
          }
        }
        return result as Plato;
      })
    );
  }

  deletePlato(id: number): Observable<boolean> {
    return this.api.delete<boolean>(`/platos/${id}`);
  }
}
