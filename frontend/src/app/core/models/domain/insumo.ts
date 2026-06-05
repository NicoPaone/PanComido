import { UnidadMedida } from './unidad-medida';
import { CategoriaInsumo } from './categoria-insumo';

export interface Insumo {
  id: number;
  nombre: string;
  stockActual: number;
  vencimiento: string;
  unidadMedida: UnidadMedida;
  categoriaIngrediente: CategoriaInsumo;
  stockMinimo: number;
}
