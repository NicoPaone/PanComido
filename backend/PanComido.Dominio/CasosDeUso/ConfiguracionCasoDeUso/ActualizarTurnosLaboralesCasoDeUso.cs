using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces;
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
            return await _turnoLaboralRepositorio.ActualizarTurnosLaboralesAsync(restauranteId, turnosLaborales);

        }
    }
}
