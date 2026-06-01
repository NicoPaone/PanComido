import { UnidadMedida } from '../models/producto-stock';

export type EstadoPedidoProveedor = 'Pendiente' | 'Confirmado' | 'Recibido' | 'Cancelado';

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
  historialPedidos: PedidoProveedor[];
  categorias?: string[];
}

export interface NuevoPedidoProveedor {
  proveedorId: number | string;
  concepto: string;
  monto: number;
  observacion: string;
  items: PedidoProveedorItem[];
}

export interface NuevoProveedor {
  nombre: string;
  contacto: string;
  telefono: string;
  email: string;
  calle?: string;
  numero?: string;
  ciudad?: string;
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
  consumoEstimado30Dias: number;
  cantidadSugerida: number;
  precioUnitario: number;
}