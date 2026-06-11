import { UnidadMedida } from './unidad-medida';

export type EstadoPedidoProveedor = 'Pendiente' | 'Enviado' | 'Recibido' | 'Confirmado' | 'Cancelado';

export interface PedidoProveedor {
  id: string | number;
  fecha: string;
  concepto: string;
  monto: number;
  estado: EstadoPedidoProveedor;
  observacion: string;
  items: PedidoProveedorItem[];
}

export interface PedidoProveedorItem {
  id: string | number;
  nombre: string;
  cantidad: number;
  unidadMedida: UnidadMedida;
  precioUnitario?: number;
}

export interface Proveedor {
  id: string | number;
  nombre: string;
  contacto: string;
  telefono: string;
  email: string;
  direccion: string;
  activo: boolean;
  fechaUltimoPedido: string | null;
  historialPedidos?: PedidoProveedor[];
  categorias?: string[];
}

export interface ProductoPedidoProveedor {
  id: string | number;
  nombre: string;
  unidadMedida: UnidadMedida;
}

export interface SugerenciaPedidoItem {
  productoId: string;
  nombre: string;
  unidadMedida: UnidadMedida;
  stockActual: number;
  stockMinimo: number;
  estadoStock?: string;
  cantidadSugerida: number;
  precioUnitario: number;
}

export interface ProveedorNuevo {
  nombre: string;
  numeroTelefonoWsp?: string;
  categoriaIds: number[];
}

export interface PedidoProveedorRequest {
  proveedorId: number | string;
  concepto: string;
  monto: number;
  observacion: string;
  items: PedidoProveedorItem[];
}

export interface RecepcionPedidoItem {
  insumoId: number;
  nombreInsumo: string;
  cantidad: number;
  nombreLote: string;
  bodegaId: number;
  fechaVencimiento: string;
}
