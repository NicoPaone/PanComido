using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarTurnosLaboralesCasoDeUso
    {
        private readonly ITurnoLaboralRepositorio _turnoLaboralRepositorio;
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly ICalculadorVentanaTurnoServicio _calculadorVentanaTurnoServicio;
        private readonly ILogger<ActualizarTurnosLaboralesCasoDeUso> _logger;

        public ActualizarTurnosLaboralesCasoDeUso(
            ITurnoLaboralRepositorio turnoLaboralRepositorio,
            IPagoRepositorio pagoRepositorio,
            ICalculadorVentanaTurnoServicio calculadorVentanaTurnoServicio,
            ILogger<ActualizarTurnosLaboralesCasoDeUso> logger)
        {
            _turnoLaboralRepositorio = turnoLaboralRepositorio;
            _pagoRepositorio = pagoRepositorio;
            _calculadorVentanaTurnoServicio = calculadorVentanaTurnoServicio;
            _logger = logger;
        }

        public async Task<List<TurnoLaboral>> EjecutarAsync(int restauranteId, List<TurnoLaboral> turnosLaborales)
        {
            foreach (var turno in turnosLaborales)
            {
                if (!turno.EsNocturno && turno.HorarioInicio >= turno.HorarioFin)
                    throw new ArgumentException("El horario de inicio debe ser anterior al horario de fin.");
            }

            var turnosActuales = await _turnoLaboralRepositorio.ObtenerTurnosLaboralesAsync(restauranteId);

            foreach (var turno in turnosLaborales)
            {
                var turnoActual = turnosActuales.FirstOrDefault(t => t.Id == turno.Id);
                await ValidarSinCierrePendienteAsync(restauranteId, turnoActual, turno);
            }

            List<TurnoLaboral> resultado = await _turnoLaboralRepositorio.ActualizarTurnosLaboralesAsync(restauranteId, turnosLaborales);
            _logger.LogInformation("Turnos laborales actualizados. RestauranteId: {RestauranteId}, CantidadTurnos: {CantidadTurnos}", restauranteId, turnosLaborales.Count);
            return resultado;
        }

        private async Task ValidarSinCierrePendienteAsync(int restauranteId, TurnoLaboral? turnoActual, TurnoLaboral turnoNuevo)
        {
            bool horarioCambio = turnoActual != null &&
                (turnoActual.HorarioInicio != turnoNuevo.HorarioInicio || turnoActual.HorarioFin != turnoNuevo.HorarioFin);

            if (!horarioCambio) return;

            (DateTime Inicio, DateTime Fin) ventana;
            try
            {
                ventana = _calculadorVentanaTurnoServicio.CalcularVentana(turnoActual!, DateTime.Now);
            }
            catch (InvalidOperationException)
            {
                return;
            }

            var pagosPendientes = await _pagoRepositorio.ObtenerPagosParaCierreAsync(restauranteId, ventana.Inicio, ventana.Fin);
            if (pagosPendientes.Any())
                throw new InvalidOperationException("No se puede cambiar el horario: hay un cierre pendiente para este turno.");
        }
    }
}
