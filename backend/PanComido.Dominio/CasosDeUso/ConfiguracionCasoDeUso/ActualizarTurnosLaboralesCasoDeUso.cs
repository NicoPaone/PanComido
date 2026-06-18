using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarTurnosLaboralesCasoDeUso
    {
        private readonly ITurnoLaboralRepositorio _turnoLaboralRepositorio;

        public ActualizarTurnosLaboralesCasoDeUso(ITurnoLaboralRepositorio turnoLaboralRepositorio)
        {
            _turnoLaboralRepositorio = turnoLaboralRepositorio;
        }

        public async Task<List<TurnoLaboral>> EjecutarAsync(int restauranteId, List<TurnoLaboral> turnosLaborales)
        {
            foreach (var turno in turnosLaborales)
            {
                if (!turno.EsNocturno && turno.HorarioInicio >= turno.HorarioFin)
                    throw new ArgumentException("El horario de inicio debe ser anterior al horario de fin.");
            }
            return await _turnoLaboralRepositorio.ActualizarTurnosLaboralesAsync(restauranteId, turnosLaborales);

        }
    }
}
