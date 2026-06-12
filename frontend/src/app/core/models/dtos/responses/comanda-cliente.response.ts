export interface ComandaClienteResponse {
  comandaId: number;
  estadoUI: string;
  totalAPagar: number;
  items: ItemComandaCliente[];
}

export interface ItemComandaCliente {
  articuloId: number;
  nombre: string;
  cantidad: number;
  entregado: boolean;
  precioUnitario: number;
  subtotal: number;
  observacionesIngredientes: string | null;
  observacionesGenerales: string | null;
}
