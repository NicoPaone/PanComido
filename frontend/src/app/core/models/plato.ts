import { UnidadMedida } from '../models/producto-stock';

export interface RecetaIngrediente {
  id: number | string;
  nombre: string;
  cantidad: number;
  unidadMedida: UnidadMedida;
}

export interface Plato {
  id: number;
  nombre: string;
  precioVenta: number;
  costo: number;
  visible: boolean;
  imagen: string;
  receta?: RecetaIngrediente[];
  tiempoPreparacion?: number;
  categoria?: string;
  recomendado?: boolean;
  ventas?: number;
}
