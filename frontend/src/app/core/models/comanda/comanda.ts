
export type EstadoComanda = 'Nueva' | 'EnPreparacion' | 'EnEspera' | 'Finalizada';

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
  horaInicio: string; 
  horaFin: string | null;
  tiempoEstimadoTotal: number;
  platos: PlatoComanda[];
}