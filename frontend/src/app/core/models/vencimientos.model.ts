import { UnidadMedida } from "./unidad-medida";

export interface IngredienteVencimiento {
  id: string | number;
  nombre: string;
  fechaVencimiento: string;
  stockDisponible: number;
  unidadMedida: UnidadMedida;
}

export interface VencimientoProveedor {
  id: string | number;
  nombre: string;
}

export interface VencimientoPedidoActivo {
  id: string;
  numeroEnvio: string;
  fechaCreacion: string;
}
