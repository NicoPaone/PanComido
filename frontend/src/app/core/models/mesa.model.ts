export enum FormaMesa {
  Redonda = 'redonda',
  Cuadrada = 'cuadrada',
  HorizontalLarga = 'horizontal_larga',
  HorizontalAlta = 'horizontal_alta'
}

export interface DimensionMesa {
  id: number;
  forma: FormaMesa;
  imagen: string;
}

export enum EstadoMesa {
  Disponible = 'disponible',
  Ocupada = 'ocupada',
  Reservada = 'reservada',
  Deshabilitada = 'deshabilitada'
}

export interface Mesa {
  id: number;
  codigoInvitacion: string;
  cantidadPersonasMax: number;
  numeroMesa: number;
  posicionYFin: number;
  posicionYinicio: number;
  posicionXInicio: number;
  posicionXfin: number;
  dimensionMesa: DimensionMesa;
  estadoMesa: EstadoMesa;
}
