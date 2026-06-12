export interface InsumoStockCriticoDto {
  id: number;
  nombre: string;
  stockActual: number;
  unidadMedida: string;
  vencimiento?: string | null;
  stockMinimo: number;
  precioVentaFinal?: number;
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
