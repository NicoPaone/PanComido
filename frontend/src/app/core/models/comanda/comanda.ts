export type EstadoComanda = 'Nueva' | 'EnPreparacion' | 'EnEspera' | 'Finalizada';

export enum EstadoComandaId {
  Nueva = 1,
  EnPreparacion = 2,
  EnEspera = 3,
  Finalizada = 4,
  Abierta = 5,
}

// Mapa para convertir string → number
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
