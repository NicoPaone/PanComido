import { InsumoResponseDto } from '../../../core/models/dtos/responses/insumo.response';
import { Insumo } from '../../../core/models/domain/insumo';

export function mapInsumoDtoToDomain(dto: InsumoResponseDto): Insumo {
  return {
    id: dto.id,
    nombre: dto.nombre,
    stockActual: dto.stockActual,
    stockMinimo: dto.stockMinimo,
    vencimiento: dto.vencimiento ?? '',
    unidadMedida: { id: 0, nombre: dto.unidadMedida }, 
    categoriaIngrediente: { 
      id: 0, 
      descripcion: dto.categoria, 
      tipoAplica: dto.tipo 
    }
  };
}
