import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { Plato } from '../../../../core/models/plato';
import { PlatoService } from '../../../../core/services/plato.service';

@Injectable({ providedIn: 'root' })
export class ModificarCartaApiService {
  private api = inject(ApiClient);
  private endpoint = '/platos';
  private platoService = inject(PlatoService, { optional: true });

  private isMock(): boolean {
    return !!this.platoService && this.platoService.constructor.name !== 'PlatoService';
  }

  getPlatos(): Observable<Plato[]> {
    if (this.isMock()) {
      return this.platoService!.getPlatos();
    }
    return this.api.get<Plato[]>('platos');
  }

  updatePlato(id: number, data: Partial<Plato>): Observable<Plato> {
    if (this.isMock()) {
      return this.platoService!.updatePlato(id, data);
    }
    return this.api.put<Plato>(`${this.endpoint}/${id}`, data);
  }

  deletePlato(id: number): Observable<boolean> {
    if (this.isMock()) {
      return this.platoService!.deletePlato(id);
    }
    return this.api.delete<boolean>(`${this.endpoint}/${id}`);
  }
}
