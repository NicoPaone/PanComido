using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
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
    public class GenerarReporteDashboardPdfCasoDeUsoTest
    {
        private readonly Mock<IDashboardRepositorio> _mockDashboardRepositorio;
        private readonly Mock<IPlatoAnalisisRepositorio> _mockPlatoAnalisisRepositorio;
        private readonly Mock<IInsumoRepositorio> _mockInsumoRepositorio;
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        private readonly Mock<IPdfGeneradorServicio> _mockPdfGeneradorServicio;

        private readonly ObtenerResumenOperativoCasoDeUso _resumenOperativoCasoDeUso;
        private readonly ObtenerRendimientoComercialCasoDeUso _rendimientoComercialCasoDeUso;
        private readonly ObtenerVencimientosYCriticidadDashboardCasoDeUso _vencimientosCasoDeUso;
        private readonly GenerarReporteDashboardPdfCasoDeUso _casoDeUso;

        public GenerarReporteDashboardPdfCasoDeUsoTest()
        {
            _mockDashboardRepositorio = new Mock<IDashboardRepositorio>();
            _mockPlatoAnalisisRepositorio = new Mock<IPlatoAnalisisRepositorio>();
            _mockInsumoRepositorio = new Mock<IInsumoRepositorio>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockPdfGeneradorServicio = new Mock<IPdfGeneradorServicio>();

            _resumenOperativoCasoDeUso = new ObtenerResumenOperativoCasoDeUso(
                _mockDashboardRepositorio.Object, 
                _mockPlatoAnalisisRepositorio.Object);

            _rendimientoComercialCasoDeUso = new ObtenerRendimientoComercialCasoDeUso(
                _mockPlatoAnalisisRepositorio.Object);

            _vencimientosCasoDeUso = new ObtenerVencimientosYCriticidadDashboardCasoDeUso(
                _mockInsumoRepositorio.Object, 
                _mockDateTimeProvider.Object);

            _casoDeUso = new GenerarReporteDashboardPdfCasoDeUso(
                _resumenOperativoCasoDeUso,
                _rendimientoComercialCasoDeUso,
                _vencimientosCasoDeUso,
                _mockPdfGeneradorServicio.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRecopilarDatosYGenerarPdf()
        {
            // Preparar
            int restauranteId = 1;
            var desde = new DateTime(2023, 1, 1);
            var hasta = new DateTime(2023, 1, 31);
            var expectedPdfBytes = new byte[] { 7, 8, 9 };

            _mockDashboardRepositorio.Setup(x => x.ObtenerTotalesPeriodoAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TotalesPeriodo());
            
            _mockDashboardRepositorio.Setup(x => x.ObtenerVentasAgrupadasAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<PanComido.Dominio.Entidades.Enums.TipoAgrupacionTiempo>()))
                .ReturnsAsync(new List<VentaAgrupada>());

            _mockDashboardRepositorio.Setup(x => x.ObtenerEstadisticasMozosRawAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<EstadisticaMozoRaw>());

            _mockPlatoAnalisisRepositorio.Setup(x => x.ObtenerRecordatoriosActivosAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Notificacion>());
                
            _mockPlatoAnalisisRepositorio.Setup(x => x.ObtenerTopPlatosMasVendidosAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new List<RendimientoPlato>());
                
            _mockPlatoAnalisisRepositorio.Setup(x => x.ObtenerTopPlatosMenosVendidosAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new List<RendimientoPlato>());

            _mockInsumoRepositorio.Setup(x => x.ObtenerInsumosProximosAVencerAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Insumo>());

            _mockDateTimeProvider.Setup(x => x.ObtenerAhora())
                .Returns(new DateTime(2023, 1, 15));

            _mockPdfGeneradorServicio.Setup(x => x.GenerarReporteDashboard(
                    It.IsAny<ResumenOperativo>(), 
                    It.IsAny<ResumenRendimientoComercial>(), 
                    It.IsAny<List<Insumo>>(), 
                    desde, 
                    hasta))
                .Returns(expectedPdfBytes);

            // Actuar
            var result = await _casoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            // Verificar
            Assert.Equal(expectedPdfBytes, result);
            _mockPdfGeneradorServicio.Verify(x => x.GenerarReporteDashboard(
                It.IsAny<ResumenOperativo>(), 
                It.IsAny<ResumenRendimientoComercial>(), 
                It.IsAny<List<Insumo>>(), 
                desde, 
                hasta), Times.Once);
        }
    }
}
