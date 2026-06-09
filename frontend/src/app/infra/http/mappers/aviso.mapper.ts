import { Aviso, InsumoStockCritico, LoteVencimiento } from '../../../core/models/domain/aviso';
import { AvisosResponseDto, InsumoStockCriticoDto, LoteVencimientoDto } from '../../../core/models/dtos/responses/avisos.response';
import { Insumo } from '../../../core/models/domain/insumo';

export function mapAvisosResponseToDomain(dto: AvisosResponseDto, insumosMap: Map<number, Insumo>): { vencimientos: Aviso[], stockBajo: Aviso[] } {
  
  const stockBajo: Aviso[] = dto.insumosConStockCritico.map((insumo: InsumoStockCriticoDto) => {
    const payload: InsumoStockCritico = {
      id: insumo.id,
      nombre: insumo.nombre,
      stockActual: insumo.stockActual,
      unidadMedida: insumo.unidadMedida,
      vencimiento: insumo.vencimiento,
      stockMinimo: insumo.stockMinimo,
      precioVentaFinal: insumo.precioVentaFinal ?? 0,
      estadoStock: insumo.estadoStock,
      tipo: insumo.tipo,
      categoria: insumo.categoria,
    };
    return {
      id: insumo.id.toString(),
      tipo: 'stock' as const,
      titulo: insumo.nombre || 'Insumo sin nombre',
      subtitulo: `Stock: ${insumo.stockActual} ${insumo.unidadMedida}`,
      info: `Punto mínimo: ${insumo.stockMinimo} ${insumo.unidadMedida}`,
      payloadStock: payload
    };
  });

  const vencimientos: Aviso[] = [];
  Object.entries(dto.insumosConVencimientoProximo).forEach(([insumoIdStr, lotes]) => {
    const insumoId = Number(insumoIdStr);
    const insumoData = insumosMap.get(insumoId);
    const nombreInsumo = insumoData?.nombre || `Insumo ${insumoIdStr}`;

    lotes.forEach((lote: LoteVencimientoDto) => {
      const payload: LoteVencimiento = {
        id: lote.id,
        nombre: lote.nombre,
        insumoId: lote.insumoId,
        cantidad: lote.cantidad,
        fechaVencimiento: lote.fechaVencimiento,
        bodegaId: lote.bodegaId,
        precioVentaFinal: insumoData?.precioVentaFinal ?? 0,
      };
      vencimientos.push({
        id: lote.id.toString(),
        tipo: 'vencimiento' as const,
        titulo: lote.nombre || `Lote de ${nombreInsumo}`,
        subtitulo: `Vence: ${lote.fechaVencimiento === '0001-01-01' ? 'Sin fecha' : lote.fechaVencimiento}`,
        info: `Cantidad: ${lote.cantidad}`,
        payloadVencimiento: payload
      });
    });
  });

  return { vencimientos, stockBajo };
}
