
export type EstadoComanda = 'Nueva' | 'EnPreparacion' | 'EnEspera' | 'Finalizada';
export enum EstadoComandaId {
  Nueva = 1,
  EnPreparacion = 2,
  EnEspera = 3,
  Finalizada = 4
}

export interface PlatoComanda {
  nombre: string;
  cantidad: number;
  observaciones: string | null;
}

export interface Comanda {
  id: number;
  mesaId: number;
  cantComensales: number;
  estado: EstadoComanda;
  estadoId: EstadoComandaId;
  horaInicio: string; 
  horaFin: string | null;
  tiempoEstimadoTotal: number;
  platos: PlatoComanda[];
}