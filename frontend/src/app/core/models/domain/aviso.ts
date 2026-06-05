import { InsumoStockCriticoDto, LoteVencimientoDto } from "../dtos/responses/avisos.response";

export type AvisoTipo = 'vencimiento' | 'stock' | 'sugerencia';

export interface Aviso {
  id: string;
  tipo: AvisoTipo;
  titulo: string;
  subtitulo?: string;
  info?: string;
  payloadStock?: InsumoStockCriticoDto;
  payloadVencimiento?: LoteVencimientoDto;
}
