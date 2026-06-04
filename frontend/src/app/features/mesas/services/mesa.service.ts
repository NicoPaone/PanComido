import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Mesa, EstadoMesa } from '../../../core/models/mesa.model';
import { MesaOcuparResponse } from '../../../core/models/mesa-ocupar-response';
import { ApiService } from '../../../core/services/api-service';

@Injectable({ providedIn: 'root' })
export class MesaService {

  private api = inject(ApiService);
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
    return this.api.patch<Mesa>(`${this.endpoint}/${mesaId}/estado`, { estadoMesa });
  }
}