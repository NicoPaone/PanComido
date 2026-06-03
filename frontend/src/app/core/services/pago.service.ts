import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api-service';
import { Llamado } from '../models/llamados/llamado';

@Injectable({ providedIn: 'root' })
export class PagoService {
  private api = inject(ApiService);

  solicitarPagoEfectivo(comandaId: number): Observable<Llamado> {
    return this.api.post<Llamado>(`pago/solicitar-efectivo/${comandaId}`, {});
  }
}
