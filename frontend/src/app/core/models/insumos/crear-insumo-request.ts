
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
// {
//   "nombre": "string",
//   "descripcion": "string",
//   "precioVentaFinal": 0,
//   "stockMinimo": 0,
//   "categoriaId": 2147483647,
//   "unidadDeMedidaId": 2147483647,
//   "cantidadInicial": 2147483647,
//   "bodegaId": 2147483647,
//   "fechaVencimiento": "2026-05-30"
// }