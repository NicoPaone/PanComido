import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { CierreTurnoDto, CierreConfirmadoDto, CierreHistorialDto } from '../../../../core/models/dtos/responses/cierre-caja.response';
import { CierreCajaRequest } from '../../../../core/models/domain/cierre-caja';

@Injectable({
  providedIn: 'root'
})
export class CierreCajaApiService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getTurno(): Observable<CierreTurnoDto> {
    return this.http.get<CierreTurnoDto>(`${this.apiUrl}/api/cierre/turno`);
  }

  postCierre(request: CierreCajaRequest): Observable<CierreConfirmadoDto> {
    return this.http.post<CierreConfirmadoDto>(`${this.apiUrl}/api/cierre`, request);
  }

  getHistorial(): Observable<CierreHistorialDto[]> {
    return this.http.get<CierreHistorialDto[]>(`${this.apiUrl}/api/cierre/historial`);
  }
}
