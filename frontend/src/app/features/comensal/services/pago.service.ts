import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api-service';
import { Llamado } from '../../../core/models/domain/llamado';

@Injectable({ providedIn: 'root' })
export class PagoService {
  private api = inject(ApiService);

  solicitarPagoEfectivo(comandaId: number, restauranteId: number): Observable<Llamado> {
    return this.api.post<Llamado>(`pago/solicitar-efectivo/${comandaId}/comensal/${restauranteId}`, {});
  }
}
  