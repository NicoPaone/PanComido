export interface IngredienteVencimiento {
  id: string;
  nombre: string;
  fechaVencimiento: string;
  stockDisponible: number;
  unidadMedida: string;
}

export interface VencimientoProveedor {
  id: string;
  nombre: string;
}

export interface VencimientoPedidoActivo {
  id: string;
  numeroEnvio: string;
  fechaCreacion: string;
}
