import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../../core/services/api-service';
import { Bodega } from '../../../../../core/models/bodega/bodega';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BodegaService {
  
  
  private api = inject(ApiService);
  private endpoint = 'Bodega';

  obtenerBodegas(): Observable<Bodega[]> {
    return this.api.get<Bodega[]>(this.endpoint);
  }
  obtenerBodegaPorId(id: number): Observable<Bodega> {
    return this.api.get<Bodega>(`${this.endpoint}/${id}`);
  }



}
