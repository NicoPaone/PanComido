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
  tiempo: number;
  tipo: string;
  bebida: string;
  restriccion: string;
  visible: boolean;
  imagen: string;
  descripcion: string;
  receta?: RecetaIngrediente[];
}
