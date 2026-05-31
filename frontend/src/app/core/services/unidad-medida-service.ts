import { inject, Injectable } from '@angular/core';
import { ApiClient } from './api-client';
import { UnidadMedida } from '../models/unidad-medida';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UnidadMedidaService {

  private api = inject(ApiClient);

  private endpoint = 'unidad-medida'

  obtenerUnidades(): Observable<UnidadMedida[]> {
    return this.api.get<UnidadMedida[]>(this.endpoint);
  }
}
