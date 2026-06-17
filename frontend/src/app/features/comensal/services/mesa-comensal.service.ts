import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api-service';

@Injectable({ providedIn: 'root' })
export class MesaComensalService {

  private api = inject(ApiService);

  obtenerBienvenida(mesaId: number, restauranteId: number): Observable<any> {
    return this.api.get(
      `mesa/${mesaId}/comensal/${restauranteId}/bienvenida`
    );
  }
}