import { Insumo } from "../insumos/insumo";

export interface Bodega {
  id: number;
  nombre: string;
  tipoBodega: string;
  insumos?: Insumo[];
}