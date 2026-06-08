export interface CrearInsumoRequest {
  nombre: string;
  descripcion: string;
  precioVentaFinal: number;
  stockMinimo: number;
  categoriaId: number;
  unidadDeMedidaId: number;
  bodegaId: number;
  cantidadInicial: number;
  fechaVencimiento: string;
}
