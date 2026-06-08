import { UnidadMedida } from './unidad-medida';

export interface RecetaIngrediente {
  id: number | string;
  nombre: string;
  cantidad: number;
  unidadMedida: UnidadMedida | string;
  costoUnitario?: number;
}

export interface Plato {
  id: number;
  nombre: string;
  precioVenta: number;
  costo: number;
  tiempo?: number;
  tipoPlatoId?: number;
  categoriaPlatoId?: number;
  tipo: string;
  bebida?: string;
  restriccion?: string;
  restriccionesIds?: number[];
  visible: boolean;
  imagen: string;
  descripcion?: string;
  platoDelDia?: boolean;
  receta?: RecetaIngrediente[];
  tiempoPreparacion?: number;
  categoria?: string;
  recomendado?: boolean;
  ventas?: number;
}
