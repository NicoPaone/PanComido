using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ObtenerTurnosLaboralesCasoDeUso
    {
        private readonly ITurnoLaboralRepositorio _turnoLaboralRepositorio;

        public ObtenerTurnosLaboralesCasoDeUso(ITurnoLaboralRepositorio turnoLaboralRepositorio)
        {
            _turnoLaboralRepositorio = turnoLaboralRepositorio;
        }

        public async Task<List<TurnoLaboral>> EjecutarAsync(int restauranteId)
        {
            return await _turnoLaboralRepositorio.ObtenerTurnosLaboralesAsync(restauranteId);
        }
    }
}
