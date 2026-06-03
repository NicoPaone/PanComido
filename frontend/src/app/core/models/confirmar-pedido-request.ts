export interface ConfirmarPedidoRequest {
  items: ItemPedidoRequest[];
}

export interface ItemPedidoRequest {
  articuloId: number;
  cantidad: number;
  observacionesIngredientes: string | null;
  observacionesGenerales: string | null;
}
