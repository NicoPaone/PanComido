import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { Plato } from '../../../../core/models/plato';

@Injectable({ providedIn: 'root' })
export class CrearPlatoApiService {
  private api = inject(ApiClient);
  private endpoint = 'platos';

  crearPlato(plato: Omit<Plato, 'id'>): Observable<Plato> {
    return this.api.post<Plato>(this.endpoint, plato);
  }
}
