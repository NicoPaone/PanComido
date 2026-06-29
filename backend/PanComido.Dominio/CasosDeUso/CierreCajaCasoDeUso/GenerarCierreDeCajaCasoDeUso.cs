using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.CierreCajaCasoDeUso
{
    public class GenerarCierreDeCajaCasoDeUso
    {
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly ITurnoLaboralRepositorio _turnoLaboralRepositorio;

        public GenerarCierreDeCajaCasoDeUso(IPagoRepositorio pagoRepositorio, ITurnoLaboralRepositorio turnoLaboralRepositorio)
        {
            _pagoRepositorio = pagoRepositorio;
            _turnoLaboralRepositorio = turnoLaboralRepositorio;
        }

        public async Task<Cierre> EjecutarAsync(int restauranteId)
        {

        }

        private List<Pago> ObtenerPagos(int restauranteId,
                                        DateTime fechaInicio,
                                        DateTime fechaFin)
        {
            return _pagoRepositorio.ObtenerPagosParaCierreAsync(restauranteId, fechaInicio, fechaFin).Result;
        }
        private async Task<DateTime?> ObtenerFechaInicio(int restauranteId)
        {
            var turnos = await _turnoLaboralRepositorio.ObtenerTurnosLaboralesAsync(restauranteId);
            if (turnos == null)
            {
                return null;
            }
            var horaInicio = turnos.Single(t => !t.EsNocturno).HorarioInicio.ToTimeSpan();
            
            return DateTime.Today.Add(horaInicio);
        }
        private async Task<DateTime?> ObtenerFechaFin(int restauranteId)
        {
            var turnos = await _turnoLaboralRepositorio.ObtenerTurnosLaboralesAsync(restauranteId);
            if (turnos == null)
            {
                return null;
            }
            var horaFin = turnos.Single(t => t.EsNocturno).HorarioFin.ToTimeSpan();

            return DateTime.Today.AddDays(1).Add(horaFin);
        }
    }
}
