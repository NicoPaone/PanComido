import { inject, Injectable } from '@angular/core';
import { forkJoin, map, Observable, switchMap } from 'rxjs';
import { PlatoApiService } from '../../services/plato.api';
import { Plato } from '../../../../core/models/domain/plato';
import { ApiService } from '../../../../core/services/api-service';
import { InsumoResponseDto } from '../../../../core/models/dtos/responses/insumo.response';

@Injectable({ providedIn: 'root' })
export class PlatoDiaApiService {
  private api = inject(ApiService);
  private platoApi = inject(PlatoApiService);

  getPlatosConReceta(): Observable<Plato[]> {
    return this.platoApi.getPlatos().pipe(
      switchMap(platos => {
        if (platos.length === 0) {
          return forkJoin([]);
        }
        const detailRequests = platos.map(p => this.platoApi.getPlatoById(p.id));
        return forkJoin(detailRequests);
      })
    );
  }

  getInsumos(): Observable<InsumoResponseDto[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo');
  }
}
