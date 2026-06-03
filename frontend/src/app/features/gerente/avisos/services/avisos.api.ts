import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { Plato } from '../../../../core/models/plato';
import { AvisosResponseDto } from '../../../../core/models/aviso.model';
import { Insumo } from '../../../../core/models/insumos/insumo';
import { SugerenciaIA } from '../../../../core/models/sugerencia-ia.model';

@Injectable({ providedIn: 'root' })
export class AvisosApiService {
  private api = inject(ApiClient);

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
