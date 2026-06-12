export interface DetallePlatoResponse {
  id: number;
  nombre: string;
  precioVentaFinal: number;
  ingredientes: {
    insumoId: number;
    nombre: string;
    opcional: boolean;
  }[];
}