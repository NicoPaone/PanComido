export interface CierreTurnoDto {
  fecha: string;
  turnoLaboralId: number;
  nombreTurno: string;
  resumenFinanciero: {
    efectivoEsperado: number;
    totalRecaudado: number;
    totalOperaciones: number;
  };
  desglosePagos: {
    metodoPagoId: number;
    nombre: string;
    esperado: number;
    operaciones: number;
  }[];
  rendimientoTurno: {
    platosMasVendidos: {
      nombre: string;
      cantidad: number;
      total: number;
    }[];
    insumosMasUsados: {
      nombre: string;
      cantidad: number;
      unidad: string;
    }[];
  };
}

export interface CierreConfirmadoDto {
  cierreId: number;
  mensaje: string;
}

export interface CierreHistorialDto {
  id: number;
  fecha: string;
  turno: string;
  total: number;
  diferencia: number;
  estado: string;
}
