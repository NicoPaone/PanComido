export interface CierreTurnoResumen {
  efectivoEsperado: number;
  totalRecaudado: number;
  totalOperaciones: number;
}

export interface MetodoPagoInfo {
  metodoPagoId: number;
  nombre: string;
  esperado: number;
  operaciones: number;
}

export interface RendimientoPlato {
  nombre: string;
  cantidad: number;
  total: number;
}

export interface RendimientoInsumo {
  nombre: string;
  cantidad: number;
  unidad: string;
}

export interface CierreTurnoEncuesta {
  calificacionPromedio: number;
  totalEncuestas: number;
  comentarioDestacado: string;
}

export interface CierreTurnoRendimiento {
  platosMasVendidos: RendimientoPlato[];
  insumosMasUsados: RendimientoInsumo[];
  // Mocked for frontend logic:
  platosMenosVendidos?: RendimientoPlato[];
  tiempoPromedioComandas?: number; // en minutos
  cantidadComensales?: number;
  resumenEncuestas?: CierreTurnoEncuesta;
}

export interface CierreTurnoInfo {
  fecha: string;
  turnoLaboralId: number;
  nombreTurno: string;
  horario?: string; // Franja horaria que cubre
  resumenFinanciero: CierreTurnoResumen;
  desglosePagos: MetodoPagoInfo[];
  rendimientoTurno: CierreTurnoRendimiento;
}

export interface CierreCajaRequest {
  restauranteId: number;
  turnoLaboralId: number;
  efectivoContado: number;
  diferencia: number;
  sobrante: number;
  observacion: string;
}

export interface CierreHistorial {
  id: number;
  fecha: string;
  turno: string;
  total: number;
  diferencia: number;
  estado: string;
}
