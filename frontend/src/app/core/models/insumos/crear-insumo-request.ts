import { UnidadMedida } from "./insumo";

export interface CrearInsumoRequest {
  nombre: string;
  unidadMedida: UnidadMedida | string;
  stockMinimo: number;
  tipo: string;
  categoria: string;
  stockInicial: number;
  bodegaId: number;
}