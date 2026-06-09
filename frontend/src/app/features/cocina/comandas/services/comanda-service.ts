import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../core/services/api-service';
import { Observable } from 'rxjs';
import { Comanda } from '../../../../core/models/domain/comanda';

@Injectable({
  providedIn: 'root',
})
export class ComandaService {

  private api = inject(ApiService)
  private endpoint = 'comanda';

  obtenerComandasActivas(): Observable<Comanda[]> {
    return this.api.get<Comanda[]>(`${this.endpoint}/activas`);

  }
  marcarItemEntregado(comandaId: number, articuloComandaId: number): Observable<Comanda> {
    return this.api.put<Comanda>(`${this.endpoint}/${comandaId}/item/${articuloComandaId}/entregar`);
  }

  modificarEstadoComanda(comandaId: number, tipoId: number): Observable<Comanda> {
    return this.api.put<Comanda>(`${this.endpoint}/activas/${comandaId}/${tipoId}`);
  }



}
