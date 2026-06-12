export interface TurnoLaboral {
  id: number;
  restauranteId: number;
  horarioInicio: string;  // "HH:mm" — viene así serializado desde el backend
  horarioFin: string;     // "HH:mm"
  esNocturno: boolean;
}