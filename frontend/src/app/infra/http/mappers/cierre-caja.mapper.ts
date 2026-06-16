import { CierreHistorial, CierreTurnoInfo } from '../../../core/models/domain/cierre-caja';
import { CierreHistorialDto, CierreTurnoDto } from '../../../core/models/dtos/responses/cierre-caja.response';

export class CierreCajaMapper {
  static toDomainTurnoInfo(dto: CierreTurnoDto): CierreTurnoInfo {
    // Mocks agregados para satisfacer los requerimientos del frontend
    const mockedPlatosMenosVendidos = [
      { nombre: 'Ensalada Mixta', cantidad: 3, total: 15000 },
      { nombre: 'Agua sin gas', cantidad: 5, total: 5000 },
      { nombre: 'Sopa de verduras', cantidad: 6, total: 18000 },
      { nombre: 'Jugo de pomelo', cantidad: 8, total: 16000 },
      { nombre: 'Helado de limón', cantidad: 9, total: 27000 }
    ];
    
    const getHorario = (id: number) => {
      if (id === 1) return '07:00 hs a 13:00 hs';
      if (id === 3) return '13:00 hs a 19:30 hs';
      return '19:30 hs a 01:00 hs';
    };

    return {
      fecha: dto.fecha,
      turnoLaboralId: dto.turnoLaboralId,
      nombreTurno: dto.nombreTurno,
      horario: getHorario(dto.turnoLaboralId),
      resumenFinanciero: dto.resumenFinanciero,
      desglosePagos: dto.desglosePagos,
      rendimientoTurno: {
        platosMasVendidos: dto.rendimientoTurno.platosMasVendidos,
        insumosMasUsados: dto.rendimientoTurno.insumosMasUsados,
        platosMenosVendidos: mockedPlatosMenosVendidos,
        tiempoPromedioComandas: 14.5,
        cantidadComensales: Math.floor(dto.resumenFinanciero.totalOperaciones * 2.5),
        resumenEncuestas: {
          calificacionPromedio: 4.8,
          totalEncuestas: Math.floor(dto.resumenFinanciero.totalOperaciones * 0.4),
          comentarioDestacado: 'Excelente servicio y la comida llegó muy rápido.'
        }
      }
    };
  }

  static toDomainHistorialList(dtos: CierreHistorialDto[]): CierreHistorial[] {
    return dtos.map(dto => ({
      id: dto.id,
      fecha: dto.fecha,
      turno: dto.turno,
      total: dto.total,
      diferencia: dto.diferencia,
      estado: dto.estado
    }));
  }
}
