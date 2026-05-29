import { UnidadMedida } from './insumos/insumo';

export interface RecetaIngrediente {
  id: number | string;
  nombre: string;
  cantidad: number;
  unidadMedida: UnidadMedida | string;
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
}
