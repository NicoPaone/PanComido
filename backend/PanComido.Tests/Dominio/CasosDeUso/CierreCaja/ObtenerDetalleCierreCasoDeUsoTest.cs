using Moq;
using PanComido.Dominio.CasosDeUso.CierreCajaCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.CierreCaja
{
    public class ObtenerDetalleCierreCasoDeUsoTest
    {
        private readonly Mock<IPagoRepositorio> _pagoRepoMock;
        private readonly Mock<ITurnoLaboralRepositorio> _turnoLaboralRepoMock;

        public ObtenerDetalleCierreCasoDeUsoTest()
        {
            _pagoRepoMock = new Mock<IPagoRepositorio>();
            _turnoLaboralRepoMock = new Mock<ITurnoLaboralRepositorio>();
        }

        private ObtenerDetalleCierreCasoDeUso CrearCasoDeUso() =>
            new ObtenerDetalleCierreCasoDeUso(_pagoRepoMock.Object, _turnoLaboralRepoMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTurnoNoExiste_LanzaKeyNotFoundException()
        {
            var cierre = new DOM.Cierre { CierreId = 1, TurnoLaboralId = 99 };
            _turnoLaboralRepoMock.Setup(r => r.ObtenerTurnosLaboralesAsync(It.IsAny<int>())).ReturnsAsync(new List<DOM.TurnoLaboral>());
            _pagoRepoMock.Setup(r => r.ObtenerPagosPorCierreIdAsync(It.IsAny<int>())).ReturnsAsync(new List<DOM.Pago>());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(1, cierre));
        }

        [Fact]
        public async Task EjecutarAsync_UsaLosTotalesPersistidosEnElCierreYCuentaPagosPorMetodo()
        {
            var turno = new DOM.TurnoLaboral { Id = 5, EsNocturno = true };
            var cierre = new DOM.Cierre
            {
                CierreId = 1,
                TurnoLaboralId = 5,
                TotalEfectivo = 1500m,
                TotalTarjeta = 2000m,
                TotalTransferencia = 300m,
                TotalMercadoPago = 400m
            };
            var pagos = new List<DOM.Pago>
            {
                new() { PagoId = 1, MetodoDePago = MetodoPago.Efectivo, Total = 1000m },
                new() { PagoId = 2, MetodoDePago = MetodoPago.Efectivo, Total = 500m },
                new() { PagoId = 3, MetodoDePago = MetodoPago.Tarjeta, Total = 2000m }
            };

            _turnoLaboralRepoMock.Setup(r => r.ObtenerTurnosLaboralesAsync(1)).ReturnsAsync(new List<DOM.TurnoLaboral> { turno });
            _pagoRepoMock.Setup(r => r.ObtenerPagosPorCierreIdAsync(1)).ReturnsAsync(pagos);

            var (turnoResultado, cantidadTotal, totalRecaudado, resumen) = await CrearCasoDeUso().EjecutarAsync(1, cierre);

            Assert.Equal(turno, turnoResultado);
            Assert.Equal(3, cantidadTotal);
            Assert.Equal(4200m, totalRecaudado);

            var efectivo = resumen.Single(r => r.Metodo == MetodoPago.Efectivo);
            Assert.Equal(2, efectivo.CantidadPagos);
            Assert.Equal(1500m, efectivo.Total);

            var tarjeta = resumen.Single(r => r.Metodo == MetodoPago.Tarjeta);
            Assert.Equal(1, tarjeta.CantidadPagos);
            Assert.Equal(2000m, tarjeta.Total);
        }
    }
}
