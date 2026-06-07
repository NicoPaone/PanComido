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
  precioVentaFinal?: number;
}

export interface CrearInsumo {
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

export interface LoteInsumo {
  id: number;
  nombre: string;
  insumoId: number;
  cantidad: number;
  fechaVencimiento: string | null;
  bodegaId: number;
}
