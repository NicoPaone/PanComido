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
            // Ajustar 'hasta' para que incluya todo el día hasta las 23:59:59
            DateTime hastaAjustado = hasta.Date.AddDays(1).AddTicks(-1);

            var masVendidos = await _comandaRepositorio.ObtenerTopPlatosMasVendidosAsync(restauranteId, desde, hastaAjustado, 5);
            var menosVendidos = await _comandaRepositorio.ObtenerTopPlatosMenosVendidosAsync(restauranteId, desde, hastaAjustado, 5);

            return new ResumenRendimientoComercial
            {
                MasVendidos = masVendidos,
                MenosVendidos = menosVendidos
            };
        }
    }
}
