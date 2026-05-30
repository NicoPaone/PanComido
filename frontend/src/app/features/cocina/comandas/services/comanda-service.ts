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



}
