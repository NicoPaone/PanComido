import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { Plato } from '../../../../core/models/domain/plato';
import { AvisosResponseDto } from '../../../../core/models/dtos/responses/avisos.response';
import { Insumo } from '../../../../core/models/domain/insumo';
import { SugerenciaIA } from '../../../../core/models/dtos/responses/sugerencia-ia.response';

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

  generarSugerenciasIA(): Observable<SugerenciaIA> {
    return this.api.post<SugerenciaIA>('avisos/sugerencias-ia', {});
  }

  crearPlatoDesdeIA(plato: any): Observable<any> {
  return this.api.post<any>('plato', plato);
}
}
