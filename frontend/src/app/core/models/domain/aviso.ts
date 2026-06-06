export type AvisoTipo = 'vencimiento' | 'stock' | 'sugerencia';

export interface InsumoStockCritico {
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

export interface LoteVencimiento {
  id: number;
  nombre: string;
  insumoId: number;
  cantidad: number;
  fechaVencimiento: string;
  bodegaId: number;
}

export interface Aviso {
  id: string;
  tipo: AvisoTipo;
  titulo: string;
  subtitulo?: string;
  info?: string;
  payloadStock?: InsumoStockCritico;
  payloadVencimiento?: LoteVencimiento;
}
