import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Mesa } from '../models/mesa.model';

@Injectable({ providedIn: 'root' })
export class MesaService {
  private http = inject(HttpClient);
  private url = '/api/mesas';

  getMesas(): Observable<Mesa[]> {
    return this.http.get<Mesa[]>(this.url);
  }

  // Usamos Partial<Mesa> para mandar solo lo que cambia (ej. las coordenadas)
  actualizarMesa(id: number, mesaParcial: Partial<Mesa>): Observable<Mesa> {
    return this.http.put<Mesa>(`${this.url}/${id}`, mesaParcial);
  }
}
