import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DetallePlatoResponse } from '../../../core/models/dtos/responses/detalle-plato-response'
import { environment } from '../../../../environments/environment.development';




@Injectable({
  providedIn: 'root'
})
export class PlatoService {

  private http = inject(HttpClient);

  getPlatoDetalle(id: number) {
  return this.http.get<DetallePlatoResponse>(
    `${environment.apiUrl}/plato/${id}?restauranteId=1`
  );
}
}

