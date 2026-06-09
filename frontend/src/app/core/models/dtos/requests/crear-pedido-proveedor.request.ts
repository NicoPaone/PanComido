export interface CrearPedidoProveedorRequestDto {
  items: {
    insumoId: number;
    cantidad: number;
    precioCompra: number;
  }[];
}
