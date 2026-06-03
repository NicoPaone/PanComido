export interface MesaOcuparResponse {
  mesa: MesaInfo;
  idComandaGenerada: number;
}

export interface MesaInfo {
  id: number;
  numeroMesa: number;
  cantidadPersonasMax: number;
  estadoMesa: string;
  posicionXInicio: number;
  posicionXFin: number;
  posicionYInicio: number;
  posicionYFin: number;
  codigoInvitacion: string;
  dimensionMesa: {
    id: number;
    forma: string;
    imagen: string | null;
  };
}
