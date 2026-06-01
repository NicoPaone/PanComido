import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Mesa } from '../models/mesa.model';
import { ApiService } from './api-service';

@Injectable({ providedIn: 'root' })
export class MesaService {
  
  private api = inject(ApiService);
  private endpoint = 'mesa';

  getMesas(): Observable<Mesa[]> {
    return this.api.get<Mesa[]>(this.endpoint);
  }

  // Usamos Partial<Mesa> para mandar solo lo que cambia (ej. las coordenadas)
  actualizarMesa(id: number, mesaParcial: Partial<Mesa>): Observable<Mesa> {
    return this.api.put<Mesa>(`${this.endpoint}/${id}`, mesaParcial);
  }
}
