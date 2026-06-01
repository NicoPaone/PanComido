import { inject, Injectable } from '@angular/core';
import { ApiClient } from '../api-client';
import { Llamado, LlamarMozoRequest } from '../../models/llamados/llamado';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LlamadoService {

  private api = inject(ApiClient)
  private endpoint = 'llamado'

  crearLlamado(request: LlamarMozoRequest):Observable<{mensaje:string}>{
    return this.api.post<{mensaje:string}>(`${this.endpoint}/generar-llamado`, request);

  }
  listarPendientesDelMozo(mozoId:number): Observable<Llamado[]>{
    return this.api.get<Llamado[]>(`${this.endpoint}/ver-pendientes/${mozoId}`);
  }
  resolver(llamadoId:number): Observable<{mensaje:string}>{
    return this.api.post<{mensaje:string}>(`${this.endpoint}/resolver/${llamadoId}`, {});
  }

}
