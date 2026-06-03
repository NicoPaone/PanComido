import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { Mesa, EstadoMesa } from '../models/mesa.model';
import { MesaOcuparResponse } from '../models/mesa-ocupar-response';
import { ApiService } from './api-service';

@Injectable({ providedIn: 'root' })
export class MesaService {

  private api = inject(ApiService);
  private http = inject(HttpClient);
  private endpoint = 'mesa';
  getMesas(): Observable<Mesa[]> {
    return this.api.get<Mesa[]>(this.endpoint);
  }

  ocuparMesa(mesaId: number, cantidadComensales: number): Observable<MesaOcuparResponse> {
    return this.api.post<MesaOcuparResponse>(`${this.endpoint}/${mesaId}/ocupar`, { cantidadComensales });
  }

  reservarMesa(mesaId: number): Observable<any> {
    return this.api.post(`${this.endpoint}/${mesaId}/reservar`, {});
  }

  guardarMapa(mesas: Mesa[]): Observable<any> {
    return this.api.put(`${this.endpoint}/mapa`, mesas);
  }

  cambiarEstado(mesaId: number, estadoMesa: EstadoMesa): Observable<Mesa> {
    return this.http.patch<Mesa>(`${environment.apiUrl}/${this.endpoint}/${mesaId}/estado`, { estadoMesa });
  }
}