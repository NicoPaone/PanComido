import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Llamado, LlamadoMozo } from '../../../core/models/domain/llamado';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LlamadoService {
  private api = inject(ApiService)
  private endpoint = 'llamado'

  crearLlamado(request: LlamadoMozo): Observable<Llamado> {
    return this.api.post<Llamado>(`${this.endpoint}/generar-llamado`, request);
  }
}
