using Moq;
using PanComido.Dominio.CasosDeUso.CierreCajaCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.CierreCaja
{
    public class GenerarCierreDeCajaCasoDeUsoTest
    {
        private readonly Mock<IPagoRepositorio> _pagoRepoMock;
        private readonly Mock<ITurnoLaboralRepositorio> _turnoLaboralRepoMock;
        private readonly Mock<ICalculadorVentanaTurnoServicio> _calculadorVentanaTurnoServicioMock;
        private readonly Mock<ICierreCajaRepositorio> _cierreCajaRepoMock;

        private readonly DOM.TurnoLaboral _turno = new() { Id = 1, EsNocturno = false, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(16, 0) };
        private readonly (DateTime Inicio, DateTime Fin) _ventana = (new DateTime(2026, 1, 5, 10, 0, 0), new DateTime(2026, 1, 5, 16, 0, 0));

        public GenerarCierreDeCajaCasoDeUsoTest()
        {
            _pagoRepoMock = new Mock<IPagoRepositorio>();
            _turnoLaboralRepoMock = new Mock<ITurnoLaboralRepositorio>();
            _calculadorVentanaTurnoServicioMock = new Mock<ICalculadorVentanaTurnoServicio>();
            _cierreCajaRepoMock = new Mock<ICierreCajaRepositorio>();

            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<DOM.TurnoLaboral> { _turno });
            _calculadorVentanaTurnoServicioMock
                .Setup(s => s.CalcularVentana(_turno, It.IsAny<DateTime>()))
                .Returns(_ventana);
            _cierreCajaRepoMock
                .Setup(r => r.CrearCierreDeCajaAsync(It.IsAny<DOM.Cierre>(), It.IsAny<List<int>>()))
                .ReturnsAsync((DOM.Cierre c, List<int> _) => { c.CierreId = 99; return c; });
        }

        private GenerarCierreDeCajaCasoDeUso CrearCasoDeUso() =>
            new GenerarCierreDeCajaCasoDeUso(
                _pagoRepoMock.Object,
                _turnoLaboralRepoMock.Object,
                _calculadorVentanaTurnoServicioMock.Object,
                _cierreCajaRepoMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTurnoNoExiste_LanzaKeyNotFoundException()
        {
            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<DOM.TurnoLaboral>());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(1, 1, 100));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTurnoEnCurso_PropagaInvalidOperationException()
        {
            _calculadorVentanaTurnoServicioMock
                .Setup(s => s.CalcularVentana(_turno, It.IsAny<DateTime>()))
                .Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => CrearCasoDeUso().EjecutarAsync(1, 1, 100));
        }

        [Fact]
        public async Task EjecutarAsync_CalculaTotalesPorMetodoYPersisteCierre()
        {
            var pagos = new List<DOM.Pago>
            {
                new() { PagoId = 1, MetodoDePago = MetodoPago.Efectivo, Total = 1000m },
                new() { PagoId = 2, MetodoDePago = MetodoPago.Efectivo, Total = 500m },
                new() { PagoId = 3, MetodoDePago = MetodoPago.Tarjeta, Total = 2000m },
                new() { PagoId = 4, MetodoDePago = MetodoPago.Transferencia, Total = 300m },
                new() { PagoId = 5, MetodoDePago = MetodoPago.MercadoPago, Total = 400m }
            };
            _pagoRepoMock
                .Setup(r => r.ObtenerPagosParaCierreAsync(1, _ventana.Inicio, _ventana.Fin))
                .ReturnsAsync(pagos);

            var cierre = await CrearCasoDeUso().EjecutarAsync(1, 1, conteoCaja: 1500m);

            Assert.Equal(1500m, cierre.TotalEfectivo);
            Assert.Equal(2000m, cierre.TotalTarjeta);
            Assert.Equal(300m, cierre.TotalTransferencia);
            Assert.Equal(400m, cierre.TotalMercadoPago);
            Assert.Equal(0m, cierre.Diferencia);
            Assert.Equal(0m, cierre.Sobrante);
            Assert.Equal(99, cierre.CierreId);

            _cierreCajaRepoMock.Verify(r => r.CrearCierreDeCajaAsync(
                It.IsAny<DOM.Cierre>(),
                It.Is<List<int>>(ids => ids.Count == 5 && ids.Contains(1) && ids.Contains(5))), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoConteoCajaMenorAlEfectivo_CalculaDiferenciaNegativaYSobranteCero()
        {
            var pagos = new List<DOM.Pago> { new() { PagoId = 1, MetodoDePago = MetodoPago.Efectivo, Total = 1000m } };
            _pagoRepoMock
                .Setup(r => r.ObtenerPagosParaCierreAsync(1, _ventana.Inicio, _ventana.Fin))
                .ReturnsAsync(pagos);

            var cierre = await CrearCasoDeUso().EjecutarAsync(1, 1, conteoCaja: 900m);

            Assert.Equal(-100m, cierre.Diferencia);
            Assert.Equal(0m, cierre.Sobrante);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoConteoCajaMayorAlEfectivo_CalculaDiferenciaYSobrantePositivos()
        {
            var pagos = new List<DOM.Pago> { new() { PagoId = 1, MetodoDePago = MetodoPago.Efectivo, Total = 1000m } };
            _pagoRepoMock
                .Setup(r => r.ObtenerPagosParaCierreAsync(1, _ventana.Inicio, _ventana.Fin))
                .ReturnsAsync(pagos);

            var cierre = await CrearCasoDeUso().EjecutarAsync(1, 1, conteoCaja: 1100m);

            Assert.Equal(100m, cierre.Diferencia);
            Assert.Equal(100m, cierre.Sobrante);
        }
    }
}
