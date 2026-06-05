export type EstadoComanda = 'Nueva' | 'EnPreparacion' | 'EnEspera' | 'Finalizada';

export enum EstadoComandaId {
  Nueva = 1,
  EnPreparacion = 2,
  EnEspera = 3,
  Finalizada = 4,
  Abierta = 5,
}

export const ESTADO_COMANDA_MAP: Record<EstadoComanda, EstadoComandaId> = {
  'Nueva': EstadoComandaId.Nueva,
  'EnPreparacion': EstadoComandaId.EnPreparacion,
  'EnEspera': EstadoComandaId.EnEspera,
  'Finalizada': EstadoComandaId.Finalizada,
};

export interface PlatoComanda {
  id: number;
  entregado: boolean;
  cantidad: number;
  observacionesGenerales: string | null;
  observacionesIngredientes: string | null;
  articulo: {
    id: number;
    nombre: string;
    urlImagen: string | null;
  };
}

export interface Comanda {
  id: number;
  mesaId: number;
  cantComensales: number;
  estado: EstadoComanda;
  horaInicio: string;
  horaFin: string | null;
  horaUltimoCambioEstado: string | null;
  tiempoEstimadoTotal: number;
  items: PlatoComanda[];
}

export interface ItemEstadoPedido {
  articuloId: number;
  nombre: string;
  cantidad: number;
  entregado: boolean;
  precioUnitario: number;
  subtotal: number;
  observacionesIngredientes: string | null;
  observacionesGenerales: string | null;
}

export interface EstadoPedido {
  comandaId: number;
  estadoUI: string;
  totalAPagar: number;
  items: ItemEstadoPedido[];
}
