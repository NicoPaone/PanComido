export interface ProveedorResponseDto {
  id: number;
  nombre: string | null;
  numeroTelefonoWsp?: string | null;
  numeroTelefonoWSP?: string | null;
  telefono?: string | null;
  email?: string | null;
  correo?: string | null;
  fechaUltimoPedido: string | null;
  categorias: string[] | null;
}

export interface PedidoInsumoResponseDto {
  insumoId: number;
  nombreInsumo: string | null;
  cantidad: number;
  precioCompra: number;
}

export interface PedidoResponseDto {
  id: number;
  fecha: string | null;
  estado: string | null;
  itemsInsumo?: PedidoInsumoResponseDto[] | null;
}

export interface CrearPedidoResponseDto extends PedidoResponseDto {
  proveedorId: number;
  proveedorNombre: string | null;
  proveedorTelefono: string | null;
}

export interface ConfirmarPedidoResponseDto {
  pedidoConfirmado: PedidoResponseDto;
  linkWpp: string;
}

export interface PreRecepcionItemResponseDto {
  insumoId: number;
  nombreInsumo: string | null;
  cantidad: number;
  nombreLote: string | null;
  bodegaIdSug: number;
  fechaVencimientoSug: string | null;
}
