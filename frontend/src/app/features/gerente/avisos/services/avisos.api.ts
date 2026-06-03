import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { Plato } from '../../../../core/models/plato';
import { AvisosResponseDto } from '../../../../core/models/aviso.model';
import { Insumo } from '../../../../core/models/insumos/insumo';

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
}
