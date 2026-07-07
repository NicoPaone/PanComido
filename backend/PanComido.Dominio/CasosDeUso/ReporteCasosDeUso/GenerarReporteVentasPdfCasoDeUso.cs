using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.ReporteCasosDeUso
{
    public class GenerarReporteVentasPdfCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly IPdfGeneradorServicio _pdfGenerador;

        public GenerarReporteVentasPdfCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            IPdfGeneradorServicio pdfGenerador)
        {
            _comandaRepositorio = comandaRepositorio;
            _pdfGenerador = pdfGenerador;
        }

        public async Task<byte[]> EjecutarAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            // Ajustar fecha fin al final del día
            DateTime hastaAjustado = hasta.Date.AddDays(1).AddTicks(-1);

            var ventas = await _comandaRepositorio.ObtenerReporteVentasPorPeriodoAsync(restauranteId, desde, hastaAjustado);
            return _pdfGenerador.GenerarReporteVentas(ventas, desde, hasta);
        }
    }
}
