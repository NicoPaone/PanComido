export interface InsumoResponseDto {
  id: number;
  nombre: string;
  stockActual: number;
  stockMinimo: number;
  vencimiento: string | null;
  unidadMedida: string;
  categoria: string;
  tipo: string;
  estadoStock: string;
}
