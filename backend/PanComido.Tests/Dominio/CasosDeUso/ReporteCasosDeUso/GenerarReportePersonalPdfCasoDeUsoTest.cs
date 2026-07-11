using Moq;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.CasosDeUso.ReporteCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.ReporteCasosDeUso
{
    public class GenerarReportePersonalPdfCasoDeUsoTest
    {
        private readonly Mock<IEmpleadoRepositorio> _mockEmpleadoRepositorio;
        private readonly Mock<IPdfGeneradorServicio> _mockPdfGeneradorServicio;
        private readonly ListarEmpleadosCasoDeUso _listarEmpleadosCasoDeUso;
        private readonly GenerarReportePersonalPdfCasoDeUso _casoDeUso;

        public GenerarReportePersonalPdfCasoDeUsoTest()
        {
            _mockEmpleadoRepositorio = new Mock<IEmpleadoRepositorio>();
            _mockPdfGeneradorServicio = new Mock<IPdfGeneradorServicio>();
            _listarEmpleadosCasoDeUso = new ListarEmpleadosCasoDeUso(_mockEmpleadoRepositorio.Object);
            _casoDeUso = new GenerarReportePersonalPdfCasoDeUso(_listarEmpleadosCasoDeUso, _mockPdfGeneradorServicio.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeLlamarACasoDeUsoYGeneradorPdf()
        {
            // Preparar
            int restauranteId = 1;
            var empleados = new List<Empleado> { new Empleado { Id = 1, Nombre = "Juan" } };
            var expectedPdfBytes = new byte[] { 4, 5, 6 };

            _mockEmpleadoRepositorio.Setup(x => x.ObtenerTodosPorRestauranteAsync(restauranteId))
                .ReturnsAsync(empleados);

            _mockPdfGeneradorServicio.Setup(x => x.GenerarReportePersonal(empleados))
                .Returns(expectedPdfBytes);

            // Actuar
            var result = await _casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.Equal(expectedPdfBytes, result);
            _mockEmpleadoRepositorio.Verify(x => x.ObtenerTodosPorRestauranteAsync(restauranteId), Times.Once);
            _mockPdfGeneradorServicio.Verify(x => x.GenerarReportePersonal(empleados), Times.Once);
        }
    }
}
