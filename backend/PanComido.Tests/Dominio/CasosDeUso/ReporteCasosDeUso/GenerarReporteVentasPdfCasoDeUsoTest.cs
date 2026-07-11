using Moq;
using PanComido.Dominio.CasosDeUso.ReporteCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.ReporteCasosDeUso
{
    public class GenerarReporteVentasPdfCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _mockComandaRepositorio;
        private readonly Mock<IPdfGeneradorServicio> _mockPdfGeneradorServicio;
        private readonly GenerarReporteVentasPdfCasoDeUso _casoDeUso;

        public GenerarReporteVentasPdfCasoDeUsoTest()
        {
            _mockComandaRepositorio = new Mock<IComandaRepositorio>();
            _mockPdfGeneradorServicio = new Mock<IPdfGeneradorServicio>();
            _casoDeUso = new GenerarReporteVentasPdfCasoDeUso(_mockComandaRepositorio.Object, _mockPdfGeneradorServicio.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeLlamarAlRepositorioYGeneradorPdf()
        {
            // Preparar
            int restauranteId = 1;
            var desde = new DateTime(2023, 1, 1);
            var hasta = new DateTime(2023, 1, 31);
            var ventas = new List<VentaReporteDetalle>();
            var expectedPdfBytes = new byte[] { 1, 2, 3 };

            _mockComandaRepositorio.Setup(x => x.ObtenerReporteVentasPorPeriodoAsync(restauranteId, desde, It.IsAny<DateTime>()))
                .ReturnsAsync(ventas);

            _mockPdfGeneradorServicio.Setup(x => x.GenerarReporteVentas(ventas, desde, hasta))
                .Returns(expectedPdfBytes);

            // Actuar
            var result = await _casoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            // Verificar
            Assert.Equal(expectedPdfBytes, result);
            _mockComandaRepositorio.Verify(x => x.ObtenerReporteVentasPorPeriodoAsync(restauranteId, desde, hasta.Date.AddDays(1).AddTicks(-1)), Times.Once);
            _mockPdfGeneradorServicio.Verify(x => x.GenerarReporteVentas(ventas, desde, hasta), Times.Once);
        }
    }
}
