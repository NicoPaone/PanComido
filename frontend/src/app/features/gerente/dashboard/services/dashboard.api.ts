import { Injectable, inject } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../../core/services/api-service';
import { DashboardInsumoVencimiento, DashboardRankingItem } from '../../../../core/models/domain/dashboard';
import { DashboardVencimientoDto } from '../../../../core/models/dtos/responses/dashboard-vencimiento.response';
import { DashboardRendimientoResponseDto } from '../../../../core/models/dtos/responses/dashboard-rendimiento.response';
import { mapVencimientoDtoToDomain, mapPlatoRendimientoDtoToDomain } from '../../../../infra/http/mappers/dashboard.mapper';

@Injectable({ providedIn: 'root' })
export class DashboardApiService {
  private api = inject(ApiService);

  getVencimientos(): Observable<DashboardInsumoVencimiento[]> {
    return this.api.get<DashboardVencimientoDto[]>('gerente/dashboard/vencimientos').pipe(
      map(dtos => dtos.map(mapVencimientoDtoToDomain))
    );
  }

  getRendimientoComercial(desde: string, hasta: string): Observable<{ masVendidos: DashboardRankingItem[], menosVendidos: DashboardRankingItem[] }> {
    const params = new HttpParams()
      .set('desde', desde)
      .set('hasta', hasta);

    return this.api.get<DashboardRendimientoResponseDto>('gerente/dashboard/rendimiento', params).pipe(
      map(res => {
        const masVendidosDto = res.masVendidos || res.MasVendidos || [];
        const menosVendidosDto = res.menosVendidos || res.MenosVendidos || [];
        
        return {
          masVendidos: masVendidosDto.map(mapPlatoRendimientoDtoToDomain),
          menosVendidos: menosVendidosDto.map(mapPlatoRendimientoDtoToDomain)
        };
      })
    );
  }
}
