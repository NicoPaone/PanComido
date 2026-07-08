using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerResumenSatisfaccionCasoDeUso
    {
        private readonly IDashboardRepositorio _dashboardRepositorio;
        public ObtenerResumenSatisfaccionCasoDeUso(IDashboardRepositorio dashboardRepositorio)
        {
            _dashboardRepositorio = dashboardRepositorio;
        }
        public async Task<ResumenSatisfaccion> EjecutarAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            var encuestas = await _dashboardRepositorio.ObtenerEncuestasPorPeriodoAsync(restauranteId, desde, hasta);
            var resumen = new ResumenSatisfaccion();

            resumen.TotalEncuestas = encuestas.Count;
            if (resumen.TotalEncuestas == 0) return resumen;
            resumen.PromedioComida = encuestas.Average(e => e.PuntuacionComida);
            resumen.PromedioLugar = encuestas.Average(e => e.PuntuacionLugar);
            resumen.PromedioAtencion = encuestas.Average(e => e.PuntuacionMozo);

            resumen.TotalDerivadosGoogleMaps = encuestas.Count(e => (e.PuntuacionComida + e.PuntuacionLugar + e.PuntuacionMozo) / 3.0 >= 4.0);

            resumen.PorcentajeDerivados = Math.Round((double)resumen.TotalDerivadosGoogleMaps / resumen.TotalEncuestas * 100, 1);
            return resumen;
        }
    }
}
