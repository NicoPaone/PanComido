import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../core/services/api-service';
import { UnidadMedida } from '../../../../core/models/domain/unidad-medida';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UnidadMedidaService {

  private api = inject(ApiService);

  private endpoint = 'unidad-medida'

  obtenerUnidades(): Observable<UnidadMedida[]> {
    return this.api.get<UnidadMedida[]>(this.endpoint);
  }
}
