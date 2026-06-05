import { Aviso } from '../../../core/models/domain/aviso';
import { AvisosResponseDto, InsumoStockCriticoDto, LoteVencimientoDto } from '../../../core/models/dtos/responses/avisos.response';
import { Insumo } from '../../../core/models/domain/insumo';

export function mapAvisosResponseToDomain(dto: AvisosResponseDto, insumosMap: Map<number, Insumo>): { vencimientos: Aviso[], stockBajo: Aviso[] } {
  
  const stockBajo: Aviso[] = dto.insumosConStockCritico.map((insumo: InsumoStockCriticoDto) => ({
    id: insumo.id.toString(),
    tipo: 'stock',
    titulo: insumo.nombre || 'Insumo sin nombre',
    subtitulo: `Stock: ${insumo.stockActual} ${insumo.unidadMedida}`,
    info: `Punto mínimo: ${insumo.stockMinimo} ${insumo.unidadMedida}`,
    payloadStock: insumo
  }));

  const vencimientos: Aviso[] = [];
  Object.entries(dto.insumosConVencimientoProximo).forEach(([insumoIdStr, lotes]) => {
    const insumoId = Number(insumoIdStr);
    const insumoData = insumosMap.get(insumoId);
    const nombreInsumo = insumoData?.nombre || `Insumo ${insumoIdStr}`;

    lotes.forEach((lote: LoteVencimientoDto) => {
      vencimientos.push({
        id: lote.id.toString(),
        tipo: 'vencimiento',
        titulo: lote.nombre || `Lote de ${nombreInsumo}`,
        subtitulo: `Vence: ${lote.fechaVencimiento === '0001-01-01' ? 'Sin fecha' : lote.fechaVencimiento}`,
        info: `Cantidad: ${lote.cantidad}`,
        payloadVencimiento: lote
      });
    });
  });

  return { vencimientos, stockBajo };
}
