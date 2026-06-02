import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Observable } from 'rxjs';
import { Comanda } from '../../../core/models/comanda/comanda';

@Injectable({
  providedIn: 'root',
})
export class MozoComandaService {

  private api = inject(ApiService)
  private endpoint = 'mozo';

  listarComandas(): Observable<Comanda[]>{
    return this.api.get<Comanda[]>(`${this.endpoint}/listar-comandas`)
   }
   entregarItems(comandaId: number, articuloComandaIds: number[]): Observable<Comanda>{
    return this.api.put<Comanda>(`comanda/${comandaId}/entregar-items`, articuloComandaIds)
   }
   


}
