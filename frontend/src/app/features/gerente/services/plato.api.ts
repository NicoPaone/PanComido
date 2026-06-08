import { Injectable, inject } from '@angular/core';
import { forkJoin, map, Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api-service';
import { Plato } from '../../../core/models/domain/plato';
import { Insumo } from '../../../core/models/domain/insumo';
import { InsumoResponseDto } from '../../../core/models/dtos/responses/insumo.response';
import { CrearPlatoIngredienteDto, CrearPlatoRequestDto } from '../../../core/models/dtos/requests/crear-plato.request';
import { mapInsumoDtoToDomain } from '../../../infra/http/mappers/insumo.mapper';
import { environment } from '../../../../environments/environment.development';

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

export interface ModificarPlatoRequestDto {
  nombre: string;
  descripcion: string;
  precioVentaFinal: number;
  tiempoPreparacionBase: number;
  tipoPlatoId: number;
  categoriaPlatoId: number;
  urlImagen: string;
  esVisibleEnCarta: boolean;
  restriccionesIds: number[];
  ingredientes: CrearPlatoIngredienteDto[];
}

interface DetallePlatoResponseDto {
  id: number;
  nombre: string;
  descripcion: string;
  precioVentaFinal: number;
  tiempoPreparacionBase: number;
  tipoPlatoId: number;
  categoriaPlatoId: number;
  urlImagen: string | null;
  esVisibleEnCarta: boolean;
  restriccionesIds: number[];
  ingredientes: CrearPlatoIngredienteDto[];
}

interface PlatoArticuloBackend {
  articuloId: number;
  nombre: string;
  precioVentaFinal: number;
  costo: number;
  visibleEnCarta: boolean;
  urlImagen: string | null;
  tipoArticulo: string;
  categoria: string;
}

@Injectable({ providedIn: 'root' })
export class PlatoApiService {
  private api = inject(ApiService);
  private endpoint = 'plato';

  getDatosFormulario(): Observable<DatosFormularioCrearPlatoResponseDto> {
    return this.api.get<DatosFormularioCrearPlatoResponseDto>(`${this.endpoint}/formulario-plato`);
  }

  crearPlato(request: CrearPlatoRequestDto): Observable<Plato> {
    return this.api.post<Plato>(this.endpoint, request);
  }

  getPlatos(): Observable<Plato[]> {
    return this.api.get<PlatoArticuloBackend[]>('carta/obtener-articulos').pipe(
      map(articulos => articulos.map(dto => ({
        id: dto.articuloId,
        nombre: dto.nombre,
        precioVenta: dto.precioVentaFinal,
        costo: dto.costo,
        visible: dto.visibleEnCarta,
        imagen: dto.urlImagen ? environment.apiUrl + dto.urlImagen : '',
        tipo: dto.tipoArticulo,
        categoria: dto.categoria
      })))
    );
  }

  getPlatoById(id: number): Observable<Plato> {
    return forkJoin({
      detalle: this.api.get<DetallePlatoResponseDto>(`${this.endpoint}/${id}`),
      formulario: this.getDatosFormulario()
    }).pipe(
      map(({ detalle, formulario }) => this.mapDetalleToDomain(detalle, formulario))
    );
  }

  getInsumos(): Observable<Insumo[]> {
    return this.api.get<InsumoResponseDto[]>('Insumo').pipe(
      map(insumos => insumos.map(mapInsumoDtoToDomain))
    );
  }

  modificarPlato(id: number, request: ModificarPlatoRequestDto): Observable<Plato> {
    return this.api.put<{ mensaje: string }>(`${this.endpoint}/${id}`, request).pipe(
      map(() => this.mapModificarRequestToDomain(id, request))
    );
  }

  updatePlato(id: number, data: Partial<Plato>): Observable<Plato> {
    const payload: Record<string, unknown> = { ...data };
    if (data.visible !== undefined) {
      payload['visibleEnCarta'] = data.visible;
      delete payload['visible'];
    }
    if (data.precioVenta !== undefined) {
      payload['precioVentaFinal'] = data.precioVenta;
      delete payload['precioVenta'];
    }
    if (data.imagen !== undefined) {
      payload['urlImagen'] = data.imagen;
      delete payload['imagen'];
    }

    return this.api.patch<PlatoArticuloBackend>(`carta/articulos/${id}`, payload).pipe(
      map(dto => {
        const result: Partial<Plato> = { ...data };
        if (dto) {
          if (dto.articuloId !== undefined) result.id = dto.articuloId;
          if (dto.nombre !== undefined) result.nombre = dto.nombre;
          if (dto.precioVentaFinal !== undefined) result.precioVenta = dto.precioVentaFinal;
          if (dto.costo !== undefined) result.costo = dto.costo;
          if (dto.visibleEnCarta !== undefined) result.visible = dto.visibleEnCarta;
          if (dto.urlImagen !== undefined) result.imagen = dto.urlImagen ? environment.apiUrl + dto.urlImagen : '';
          if (dto.tipoArticulo !== undefined) result.tipo = dto.tipoArticulo;
          if (dto.categoria !== undefined) result.categoria = dto.categoria;
        }
        return result as Plato;
      })
    );
  }

  deletePlato(id: number): Observable<boolean> {
    return this.api.delete<boolean>(`/platos/${id}`);
  }

  private mapDetalleToDomain(dto: DetallePlatoResponseDto, formulario: DatosFormularioCrearPlatoResponseDto): Plato {
    const tipo = formulario.tiposPlato.find(item => item.id === dto.tipoPlatoId)?.descripcion ?? '';
    const categoria = formulario.categoriasPlato.find(item => item.id === dto.categoriaPlatoId)?.descripcion ?? '';
    const ingredientes = dto.ingredientes ?? [];

    return {
      id: dto.id,
      nombre: dto.nombre,
      descripcion: dto.descripcion,
      precioVenta: dto.precioVentaFinal,
      costo: 0,
      tiempo: dto.tiempoPreparacionBase,
      tiempoPreparacion: dto.tiempoPreparacionBase,
      tipoPlatoId: dto.tipoPlatoId,
      categoriaPlatoId: dto.categoriaPlatoId,
      tipo,
      categoria,
      restriccionesIds: dto.restriccionesIds ?? [],
      visible: dto.esVisibleEnCarta,
      imagen: dto.urlImagen ? environment.apiUrl + dto.urlImagen : '',
      receta: ingredientes.map(ingrediente => {
        const insumo = formulario.ingredientes.find(item => item.id === ingrediente.insumoId);
        return {
          id: ingrediente.insumoId,
          nombre: insumo?.nombre ?? `Insumo ${ingrediente.insumoId}`,
          cantidad: ingrediente.cantidad,
          unidadMedida: insumo?.unidadMedida ?? '',
          costoUnitario: insumo?.costoUnitario ?? 0
        };
      })
    };
  }

  private mapModificarRequestToDomain(id: number, request: ModificarPlatoRequestDto): Plato {
    return {
      id,
      nombre: request.nombre,
      descripcion: request.descripcion,
      precioVenta: request.precioVentaFinal,
      costo: 0,
      tiempo: request.tiempoPreparacionBase,
      tiempoPreparacion: request.tiempoPreparacionBase,
      tipoPlatoId: request.tipoPlatoId,
      categoriaPlatoId: request.categoriaPlatoId,
      tipo: '',
      categoria: '',
      restriccionesIds: request.restriccionesIds,
      visible: request.esVisibleEnCarta,
      imagen: request.urlImagen,
      receta: request.ingredientes.map(ingrediente => ({
        id: ingrediente.insumoId,
        nombre: '',
        cantidad: ingrediente.cantidad,
        unidadMedida: '',
        costoUnitario: 0
      }))
    };
  }
}
