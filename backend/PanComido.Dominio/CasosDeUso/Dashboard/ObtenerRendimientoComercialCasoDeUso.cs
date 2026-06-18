using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerRendimientoComercialCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;

        public ObtenerRendimientoComercialCasoDeUso(IComandaRepositorio comandaRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
        }

        public async Task<ResumenRendimientoComercial> EjecutarAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            // PostgreSQL no permite mezclar fechas UTC con columnas 'timestamp without time zone'
            DateTime desdeAjustado = DateTime.SpecifyKind(desde, DateTimeKind.Unspecified);
            DateTime hastaAjustado = DateTime.SpecifyKind(hasta.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);

            var masVendidos = await _comandaRepositorio.ObtenerTopPlatosMasVendidosAsync(restauranteId, desdeAjustado, hastaAjustado, 5);
            var menosVendidos = await _comandaRepositorio.ObtenerTopPlatosMenosVendidosAsync(restauranteId, desdeAjustado, hastaAjustado, 5);

            return new ResumenRendimientoComercial
            {
                MasVendidos = masVendidos,
                MenosVendidos = menosVendidos
            };
        }
    }
}
