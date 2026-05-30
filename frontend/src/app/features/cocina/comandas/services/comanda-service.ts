import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../core/services/api-service';
import { Comanda } from '../../../../core/models/comanda/comanda';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ComandaService {

  private api = inject(ApiService)
  private endpoint = 'Comanda';

  obtenerComandasActivas():Observable<Comanda[]>{
    return this.api.get<Comanda[]>(`${this.endpoint}/activas`);
    
  }


  //https://localhost:7204/api/Comanda/4/3 ejemplo
  modificarEstadoComanda(mesaId:number, tipoId: number):Observable<Comanda>{
    return this.api.put<Comanda>(`${this.endpoint}/activas/${mesaId}/${tipoId}`);
  }



}
