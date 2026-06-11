export interface ActualizarDatosLocalRequest {
  nombre: string;
  imagen: string | null;
  colorPrincipal: string | null;
  colorSecundario: string | null;
  textoPrincipal: string | null;
  textoSecundario: string | null;
}

export interface ActualizarMetodoPagoRequest {
  id: number;
  habilitado: boolean;
}

export interface ActualizarTurnoLaboralRequest {
  id: number;
  horarioInicio: string;
  horarioFin: string;
  esNocturno: boolean;
}