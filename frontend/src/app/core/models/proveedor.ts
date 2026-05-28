import { UnidadMedida } from '../model/producto-stock-mock';

export type EstadoPedidoProveedor = 'Pendiente' | 'Confirmado' | 'Recibido' | 'Cancelado';

export interface PedidoProveedor {
  id: number;
  fecha: string;
  concepto: string;
  monto: number;
  estado: EstadoPedidoProveedor;
  observacion: string;
  items: PedidoProveedorItem[];
}

export interface PedidoProveedorItem {
  id: string;
  nombre: string;
  cantidad: number;
  unidadMedida: UnidadMedida;
  precioUnitario?: number;
}

export interface Proveedor {
  id: number;
  nombre: string;
  contacto: string;
  telefono: string;
  email: string;
  direccion: string;
  activo: boolean;
  fechaUltimoPedido: string | null;
  historialPedidos: PedidoProveedor[];
}

export interface NuevoPedidoProveedor {
  proveedorId: number;
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
  id: string;
  nombre: string;
  unidadMedida: UnidadMedida;
}