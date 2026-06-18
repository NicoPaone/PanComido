export interface ActualizarDatosLocalRequest {
  nombre: string;
  colorPrincipal: string | null;
  colorSecundario: string | null;
  textoPrincipal: string | null;
  textoSecundario: string | null;
  familiaTipograficaId:number | null;
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
export interface ActualizarFilaVirtualRequest { 
  habilitada: boolean;
}
export interface PorcentajeItemRequest{ 
  id:number;
  porcentaje: number;
}
export interface ActualizarPorcentajeGananciaRequest{ 
  platos: PorcentajeItemRequest[];
  bebidas: PorcentajeItemRequest[];
}