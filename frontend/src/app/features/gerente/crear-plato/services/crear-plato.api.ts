import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { Plato } from '../../../../core/models/domain/plato';
import { CrearPlatoRequestDto } from '../../../../core/models/dtos/requests/crear-plato.request';

export interface ItemDesplegableDto {
  id: number;
  descripcion: string;
}

export interface IngredienteDisponibleDto {
  id: number;
  nombre: string;
  unidadMedida: string;
  costoUnitario: number;
}

export interface DatosFormularioCrearPlatoResponseDto {
  tiposPlato: ItemDesplegableDto[];
  categoriasPlato: ItemDesplegableDto[];
  restricciones: ItemDesplegableDto[];
  ingredientes: IngredienteDisponibleDto[];
}

@Injectable({ providedIn: 'root' })
export class CrearPlatoApiService {
  private api = inject(ApiService);
  private endpoint = 'plato';

  getDatosFormulario(): Observable<DatosFormularioCrearPlatoResponseDto> {
    return this.api.get<DatosFormularioCrearPlatoResponseDto>(`${this.endpoint}/formulario-plato`);
  }

  crearPlato(request: CrearPlatoRequestDto): Observable<Plato> {
    return this.api.post<Plato>(this.endpoint, request);
  }
}
