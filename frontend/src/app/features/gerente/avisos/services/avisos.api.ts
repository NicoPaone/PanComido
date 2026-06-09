import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { Plato } from '../../../../core/models/domain/plato';
import { AvisosResponseDto } from '../../../../core/models/dtos/responses/avisos.response';
import { Insumo } from '../../../../core/models/domain/insumo';
import { SugerenciaIA } from '../../../../core/models/dtos/responses/sugerencia-ia.response';
import { Sugerencia } from '../../../../core/models/domain/sugerencia-ia';

export interface CrearPlatoIARequest {
  nombre: string;
  descripcion: string;
  precioVentaFinal: number;
  tiempoPreparacionBase: number;
  tipoPlatoId: number;
  categoriaPlatoId: number;
  urlImagen: string;
  restriccionesIds: number[];
  ingredientes: { insumoId: number; cantidad: number; opcional: boolean }[];
}

@Injectable({ providedIn: 'root' })
export class AvisosApiService {
  private api = inject(ApiService);

  getAvisos(): Observable<AvisosResponseDto> {
    return this.api.get<AvisosResponseDto>('avisos');
  }

  getPlatos(): Observable<Plato[]> {
    return this.api.get<Plato[]>('platos');
  }

  updatePlato(id: number, data: Partial<Plato>): Observable<Plato> {
    return this.api.put<Plato>(`/platos/${id}`, data);
  }

  getInsumos(): Observable<Insumo[]> {
    return this.api.get<Insumo[]>('insumo');
  }

  generarSugerenciasIA(): Observable<Sugerencia> {
    return this.api.post<SugerenciaIA>('avisos/sugerencias-ia', {}).pipe(
      map((dto) => ({
        fechaSugerencia: dto.fechaSugerencia,
        platosSugeridos: dto.platosSugeridos.map((p) => ({
          id: p.id,
          nombre: p.nombre,
          descripcion: p.descripcion,
          tiempoPreparacion: p.tiempoPreparacion,
          porcionesPosibles: p.porcionesPosibles,
          ingredientesSugeridos: p.ingredientesSugeridosIA.map((ing) => ({
            insumoId: ing.insumoId,
            nombre: ing.nombre,
            cantidad: ing.cantidad,
          })),
        })),
      }))
    );
  }

  crearPlatoDesdeIA(plato: CrearPlatoIARequest): Observable<Plato> {
    return this.api.post<Plato>('plato', plato);
  }
}
