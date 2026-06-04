import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Llamado, LlamarMozoRequest } from '../../../core/models/llamados/llamado';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LlamadoService {
  private api = inject(ApiService)
  private endpoint = 'llamado'

  crearLlamado(request: LlamarMozoRequest): Observable<Llamado> {
    return this.api.post<Llamado>(`${this.endpoint}/generar-llamado`, request);
  }
}
