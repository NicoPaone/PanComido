import { PedidoProveedorItem } from '../../domain/proveedor';

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

export interface PreRecepcionPedidoItem {
  insumoId: number;
  nombreInsumo: string;
  cantidad: number;
  nombreLote: string;
  bodegaId: number;
  fechaVencimiento: string;
}
