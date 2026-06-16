import { DashboardRankingItem, DashboardInsumoVencimiento } from '../../../core/models/domain/dashboard';
import { PlatoRendimientoDto } from '../../../core/models/dtos/responses/dashboard-rendimiento.response';
import { DashboardVencimientoDto } from '../../../core/models/dtos/responses/dashboard-vencimiento.response';

export function mapPlatoRendimientoDtoToDomain(dto: PlatoRendimientoDto): DashboardRankingItem {
  const nombre = dto.nombre || dto.Nombre || '';
  const unidades = dto.unidades || dto.Unidades || "0";
  const facturacion = dto.facturacion || dto.Facturacion || '$ 0';
  
  return {
    nombre: nombre,
    valor: parseInt(String(unidades).replace(/\\D/g, '')) || 0,
    detalle: facturacion
  };
}

export function mapVencimientoDtoToDomain(dto: DashboardVencimientoDto): DashboardInsumoVencimiento {
  const criticidadCruda = (dto.criticidad || dto.Criticidad || 'baja').toLowerCase();
  let criticidadValida: 'alta' | 'media' | 'baja' = 'baja';
  if (criticidadCruda === 'alta' || criticidadCruda === 'media') {
    criticidadValida = criticidadCruda as 'alta' | 'media';
  }

  return {
    nombre: dto.nombre || dto.Nombre || '',
    fecha: dto.fecha || dto.Fecha || '',
    cantidad: dto.cantidad || dto.Cantidad || '',
    criticidad: criticidadValida,
    relativo: dto.relativo || dto.Relativo || ''
  };
}
