export interface LoteResponseDto {
  id: number;
  nombre: string;
  insumoId: number;
  cantidad: number;
  fechaVencimiento: string | null;
  bodegaId: number;
}
