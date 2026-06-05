export enum FormaMesa {
  Redonda = 'redonda',
  Cuadrada = 'cuadrada',
  HorizontalLarga = 'horizontal_larga',
  HorizontalAlta = 'horizontal_alta'
}

export interface DimensionMesa {
  id: number;
  forma: string;
  imagen?: string;
}

export enum EstadoMesa {
  Disponible = 'Disponible',
  Ocupada = 'Ocupada',
  Reservada = 'Reservada',
  Deshabilitada = 'Deshabilitada'
}

export interface Mesa {
  id: number;
  codigoInvitacion: string;
  cantidadPersonasMax: number;
  numeroMesa: number;
  posicionXInicio: number;
  posicionXFin: number;
  posicionYInicio: number;
  posicionYFin: number;
  dimensionMesa: DimensionMesa;
  estadoMesa: string;
}

export interface MesaOcupar {
  mesa: Mesa;
  idComandaGenerada: number;
}
