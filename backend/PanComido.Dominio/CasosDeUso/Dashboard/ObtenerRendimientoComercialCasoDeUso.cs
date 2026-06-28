using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
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
            DateTime desdeAjustado = DateTime.SpecifyKind(desde, DateTimeKind.Unspecified);
            DateTime hastaAjustado = DateTime.SpecifyKind(hasta.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);

            var masVendidosTask = _comandaRepositorio.ObtenerTopPlatosMasVendidosAsync(restauranteId, desdeAjustado, hastaAjustado, 5);
            var menosVendidosTask = _comandaRepositorio.ObtenerTopPlatosMenosVendidosAsync(restauranteId, desdeAjustado, hastaAjustado, 5);

            await Task.WhenAll(masVendidosTask, menosVendidosTask);

            return new ResumenRendimientoComercial
            {
                MasVendidos = await masVendidosTask,
                MenosVendidos = await menosVendidosTask
            };
        }
    }
}
