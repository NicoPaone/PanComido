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

  ocuparMesa(mesaId: number, cantidadComensales: number): Observable<any> {
    return this.api.post(`${this.endpoint}/${mesaId}/ocupar`, { cantidadComensales });
  }

  reservarMesa(mesaId: number): Observable<any> {
    return this.api.post(`${this.endpoint}/${mesaId}/reservar`, {});
  }

  guardarPosiciones(mesas: { mesaId: number; posicionXInicio: number; posicionXFin: number; posicionYInicio: number; posicionYFin: number }[]): Observable<any> {
    return this.api.put(`${this.endpoint}/posiciones`, { mesas });
  }

  crearMesa(mesa: Partial<Mesa>): Observable<Mesa> {
    return this.api.post<Mesa>(this.endpoint, mesa);
  }

  eliminarMesa(id: number): Observable<any> {
    return this.api.delete(`${this.endpoint}/${id}`);
  }
}