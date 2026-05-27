import { UnidadMedida } from '../model/producto-stock-mock';

export interface RecetaIngrediente {
  id: string;
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
}
