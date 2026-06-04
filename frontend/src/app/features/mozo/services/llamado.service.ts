import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Llamado } from '../../../core/models/llamados/llamado';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LlamadoService {
  private api = inject(ApiService)
  private endpoint = 'llamado'

  listarPendientesDelMozo(): Observable<Llamado[]>{
    return this.api.get<Llamado[]>(`${this.endpoint}/ver-pendientes`);
  }
  
  resolver(llamadoId:number): Observable<{mensaje:string}>{
    return this.api.put<{mensaje:string}>(`${this.endpoint}/resolver/${llamadoId}`, {});
  }
}
