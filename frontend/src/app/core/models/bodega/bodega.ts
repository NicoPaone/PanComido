import { Insumo } from "../insumos/insumo";

export interface Bodega {
  id: number;
  nombre: string;
  insumos?: Insumo[];
}