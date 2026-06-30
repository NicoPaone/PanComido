using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerRendimientoComercialCasoDeUso
    {
        private readonly IPlatoAnalisisRepositorio _platoAnalisisRepositorio;

        public ObtenerRendimientoComercialCasoDeUso(IPlatoAnalisisRepositorio platoAnalisisRepositorio)
        {
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
        }

        public async Task<ResumenRendimientoComercial> EjecutarAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            DateTime desdeAjustado = DateTime.SpecifyKind(desde, DateTimeKind.Unspecified);
            DateTime hastaAjustado = DateTime.SpecifyKind(hasta.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);

            var masVendidos = await _platoAnalisisRepositorio.ObtenerTopPlatosMasVendidosAsync(restauranteId, desdeAjustado, hastaAjustado, 5);
            var menosVendidos = await _platoAnalisisRepositorio.ObtenerTopPlatosMenosVendidosAsync(restauranteId, desdeAjustado, hastaAjustado, 5);

            return new ResumenRendimientoComercial
            {
                MasVendidos = masVendidos,
                MenosVendidos = menosVendidos
            };
        }
    }
}
