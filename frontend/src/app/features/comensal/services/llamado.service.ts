import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Llamado } from '../../../core/models/domain/llamado';
import { LlamarMozoRequest } from '../../../core/models/dtos/requests/llamar-mozo.request';
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
