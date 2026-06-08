import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { Plato } from '../../../../core/models/domain/plato';
import { CrearPlatoRequestDto } from '../../../../core/models/dtos/requests/crear-plato.request';

@Injectable({ providedIn: 'root' })
export class CrearPlatoApiService {
  private api = inject(ApiService);
  private endpoint = 'plato';

  crearPlato(request: CrearPlatoRequestDto): Observable<Plato> {
    return this.api.post<Plato>(this.endpoint, request);
  }
}
