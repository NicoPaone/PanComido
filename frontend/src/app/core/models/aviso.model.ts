export type AvisoTipo = 'vencimiento' | 'stock' | 'sugerencia';

export interface InsumoStockCriticoDto {
  id: number;
  nombre: string;
  stockActual: number;
  unidadMedida: string;
  vencimiento?: string | null;
  stockMinimo: number;
  estadoStock: string;
  tipo: string;
  categoria?: string | null;
}

export interface LoteVencimientoDto {
  id: number;
  nombre: string;
  insumoId: number;
  cantidad: number;
  fechaVencimiento: string;
  bodegaId: number;
}

export interface AvisosResponseDto {
  insumosConStockCritico: InsumoStockCriticoDto[];
  insumosConVencimientoProximo: Record<string, LoteVencimientoDto[]>;
}

export interface Aviso {
  id: string;
  tipo: AvisoTipo;
  titulo: string;
  subtitulo?: string;
  info?: string;
  payloadStock?: InsumoStockCriticoDto;
  payloadVencimiento?: LoteVencimientoDto;
}