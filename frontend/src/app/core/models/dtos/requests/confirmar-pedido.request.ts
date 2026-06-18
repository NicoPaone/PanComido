export interface ConfirmarPedidoRequest {
  items: ItemPedidoRequest[];
  nombreComensal: string;
}

export interface ItemPedidoRequest {
  articuloId: number;
  cantidad: number;
  observacionesIngredientes: string | null;
  observacionesGenerales: string | null;
}
