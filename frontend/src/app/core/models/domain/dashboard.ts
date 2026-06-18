export type DashboardPeriodo = '1d' | '3d' | '7d' | '30d' | '365d' | 'custom';

export type DashboardDestino = 'stock' | 'carta' | 'proveedores' | 'pedido' | 'vencimientos';

export interface DashboardRankingItem {
  nombre: string;
  valor: number;
  detalle: string;
}

export interface DashboardInsumoVencimiento {
  nombre: string;
  fecha: string;
  cantidad: string;
  criticidad: 'alta' | 'media' | 'baja';
  relativo: string;
}

export interface DashboardLecturaComercial {
  titulo: string;
  detalle: string;
  tono: 'success' | 'warning' | 'info';
}

export interface DashboardAtencionItem {
  titulo: string;
  detalle: string;
  accion: string;
  destino: DashboardDestino;
  tono: 'danger' | 'warning' | 'info';
}

export interface DashboardAccionItem {
  titulo: string;
  detalle: string;
  destino: DashboardDestino;
  tono: 'danger' | 'warning' | 'info';
  impacto: string;
  prioridad: number;
}

export interface DashboardVentaMensual {
  mes: string;
  ventas: number;
}

export interface DashboardVentaDia {
  dia: string;
  fecha: string;
  ventas: number;
}


