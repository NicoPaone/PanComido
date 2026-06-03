import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../core/services/api-service';
import { Comanda } from '../../../../core/models/comanda/comanda';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ComandaService {

  private api = inject(ApiService)
  private endpoint = 'comanda';

  obtenerComandasActivas():Observable<Comanda[]>{
    return this.api.get<Comanda[]>(`${this.endpoint}/activas`);
    
  }
  marcarItemEntregado(comandaId:number, articuloComandaId:number):Observable<Comanda>{
    return this.api.put<Comanda>(`${this.endpoint}/${comandaId}/item/${articuloComandaId}/entregar`);
  }

  //https://localhost:7204/api/Comanda/4/3 ejemplo
  modificarEstadoComanda(comandaId:number, tipoId: number):Observable<Comanda>{
    return this.api.put<Comanda>(`${this.endpoint}/activas/${comandaId}/${tipoId}`);
  }



}
