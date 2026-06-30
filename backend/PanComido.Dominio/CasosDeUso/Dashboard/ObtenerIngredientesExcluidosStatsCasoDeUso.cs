using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerIngredientesExcluidosStatsCasoDeUso
    {
        private readonly IDashboardRepositorio _dashboardRepositorio;

        public ObtenerIngredientesExcluidosStatsCasoDeUso(IDashboardRepositorio dashboardRepositorio)
        {
            _dashboardRepositorio = dashboardRepositorio;
        }

        public async Task<List<IngredienteExcluidoStat>> EjecutarAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            DateTime hastaAjustado = hasta.Date.AddDays(1).AddTicks(-1);
            return await _dashboardRepositorio.ObtenerIngredientesExcluidosStatsAsync(restauranteId, desde, hastaAjustado);
        }
    }
}
