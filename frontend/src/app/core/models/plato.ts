export interface Plato {
  id: number;
  nombre: string;
  precioVenta: number;
  costo: number;
  visible: boolean;
  imagen: string;
  tiempoPreparacion?: number; // en minutos
  categoria?: string;
}
