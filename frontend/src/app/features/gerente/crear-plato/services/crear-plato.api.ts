import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { Plato } from '../../../../core/models/domain/plato';

@Injectable({ providedIn: 'root' })
export class CrearPlatoApiService {
  private api = inject(ApiService);
  private endpoint = 'platos';

  crearPlato(plato: Omit<Plato, 'id'>): Observable<Plato> {
    return this.api.post<Plato>(this.endpoint, plato);
  }
}
