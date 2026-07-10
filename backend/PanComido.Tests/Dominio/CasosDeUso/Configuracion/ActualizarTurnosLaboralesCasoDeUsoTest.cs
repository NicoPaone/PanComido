using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ActualizarTurnosLaboralesCasoDeUsoTest
    {
        private readonly Mock<ITurnoLaboralRepositorio> _turnoLaboralRepoMock;
        private readonly Mock<IPagoRepositorio> _pagoRepoMock;
        private readonly Mock<ICalculadorVentanaTurnoServicio> _calculadorVentanaTurnoServicioMock;
        private readonly Mock<ILogger<ActualizarTurnosLaboralesCasoDeUso>> _loggerMock;

        public ActualizarTurnosLaboralesCasoDeUsoTest()
        {
            _turnoLaboralRepoMock = new Mock<ITurnoLaboralRepositorio>();
            _pagoRepoMock = new Mock<IPagoRepositorio>();
            _calculadorVentanaTurnoServicioMock = new Mock<ICalculadorVentanaTurnoServicio>();
            _loggerMock = new Mock<ILogger<ActualizarTurnosLaboralesCasoDeUso>>();

            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<DOM.TurnoLaboral>());
        }

        private ActualizarTurnosLaboralesCasoDeUso CrearCasoDeUso() =>
            new ActualizarTurnosLaboralesCasoDeUso(
                _turnoLaboralRepoMock.Object,
                _pagoRepoMock.Object,
                _calculadorVentanaTurnoServicioMock.Object,
                _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTurnosDiurnosConHorariosValidos_DevuelveListaActualizada()
        {
            int restauranteId = 1;
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = false, HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(16, 0) },
                new DOM.TurnoLaboral { Id = 2, EsNocturno = false, HorarioInicio = new TimeOnly(16, 0), HorarioFin = new TimeOnly(23, 0) }
            };
            var turnosSalida = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = false, HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(16, 0) },
                new DOM.TurnoLaboral { Id = 2, EsNocturno = false, HorarioInicio = new TimeOnly(16, 0), HorarioFin = new TimeOnly(23, 0) }
            };

            _turnoLaboralRepoMock
                .Setup(r => r.ActualizarTurnosLaboralesAsync(restauranteId, turnosEntrada))
                .ReturnsAsync(turnosSalida);

            var resultado = await CrearCasoDeUso().EjecutarAsync(restauranteId, turnosEntrada);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTurnoDiurnoConHorarioInicioMayorAlFin_LanzaArgumentException()
        {
            int restauranteId = 1;
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = false, HorarioInicio = new TimeOnly(16, 0), HorarioFin = new TimeOnly(8, 0) }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, turnosEntrada));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTurnoDiurnoConHorarioInicioIgualAlFin_LanzaArgumentException()
        {
            int restauranteId = 1;
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = false, HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(8, 0) }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, turnosEntrada));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTurnoNocturnoConHorarioInicioMayorAlFin_NoLanzaExcepcion()
        {
            int restauranteId = 1;
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = true, HorarioInicio = new TimeOnly(22, 0), HorarioFin = new TimeOnly(6, 0) }
            };
            var turnosSalida = new List<DOM.TurnoLaboral> { turnosEntrada[0] };

            _turnoLaboralRepoMock
                .Setup(r => r.ActualizarTurnosLaboralesAsync(restauranteId, turnosEntrada))
                .ReturnsAsync(turnosSalida);

            var resultado = await CrearCasoDeUso().EjecutarAsync(restauranteId, turnosEntrada);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHorarioNoCambio_NoConsultaPagosPendientes()
        {
            int restauranteId = 1;
            var turnoActual = new DOM.TurnoLaboral { Id = 1, EsNocturno = false, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(16, 0) };
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = false, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(16, 0) }
            };

            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(restauranteId))
                .ReturnsAsync(new List<DOM.TurnoLaboral> { turnoActual });
            _turnoLaboralRepoMock
                .Setup(r => r.ActualizarTurnosLaboralesAsync(restauranteId, turnosEntrada))
                .ReturnsAsync(turnosEntrada);

            await CrearCasoDeUso().EjecutarAsync(restauranteId, turnosEntrada);

            _calculadorVentanaTurnoServicioMock.Verify(
                s => s.CalcularVentana(It.IsAny<DOM.TurnoLaboral>(), It.IsAny<DateTime>()), Times.Never);
            _pagoRepoMock.Verify(
                r => r.ObtenerPagosParaCierreAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHorarioCambioYHayPagosPendientes_LanzaInvalidOperationException()
        {
            int restauranteId = 1;
            var turnoActual = new DOM.TurnoLaboral { Id = 1, EsNocturno = true, HorarioInicio = new TimeOnly(20, 0), HorarioFin = new TimeOnly(1, 0) };
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = true, HorarioInicio = new TimeOnly(21, 0), HorarioFin = new TimeOnly(2, 0) }
            };
            var ventana = (Inicio: new DateTime(2026, 1, 1, 20, 0, 0), Fin: new DateTime(2026, 1, 2, 1, 0, 0));

            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(restauranteId))
                .ReturnsAsync(new List<DOM.TurnoLaboral> { turnoActual });
            _calculadorVentanaTurnoServicioMock
                .Setup(s => s.CalcularVentana(turnoActual, It.IsAny<DateTime>()))
                .Returns(ventana);
            _pagoRepoMock
                .Setup(r => r.ObtenerPagosParaCierreAsync(restauranteId, ventana.Inicio, ventana.Fin))
                .ReturnsAsync(new List<DOM.Pago> { new DOM.Pago { PagoId = 1 } });

            await Assert.ThrowsAsync<InvalidOperationException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, turnosEntrada));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHorarioCambioYNoHayPagosPendientes_ActualizaSinLanzar()
        {
            int restauranteId = 1;
            var turnoActual = new DOM.TurnoLaboral { Id = 1, EsNocturno = true, HorarioInicio = new TimeOnly(20, 0), HorarioFin = new TimeOnly(1, 0) };
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = true, HorarioInicio = new TimeOnly(21, 0), HorarioFin = new TimeOnly(2, 0) }
            };
            var ventana = (Inicio: new DateTime(2026, 1, 1, 20, 0, 0), Fin: new DateTime(2026, 1, 2, 1, 0, 0));

            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(restauranteId))
                .ReturnsAsync(new List<DOM.TurnoLaboral> { turnoActual });
            _turnoLaboralRepoMock
                .Setup(r => r.ActualizarTurnosLaboralesAsync(restauranteId, turnosEntrada))
                .ReturnsAsync(turnosEntrada);
            _calculadorVentanaTurnoServicioMock
                .Setup(s => s.CalcularVentana(turnoActual, It.IsAny<DateTime>()))
                .Returns(ventana);
            _pagoRepoMock
                .Setup(r => r.ObtenerPagosParaCierreAsync(restauranteId, ventana.Inicio, ventana.Fin))
                .ReturnsAsync(new List<DOM.Pago>());

            var resultado = await CrearCasoDeUso().EjecutarAsync(restauranteId, turnosEntrada);

            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHorarioCambioYTurnoViejoEnCurso_ActualizaSinLanzar()
        {
            int restauranteId = 1;
            var turnoActual = new DOM.TurnoLaboral { Id = 1, EsNocturno = true, HorarioInicio = new TimeOnly(20, 0), HorarioFin = new TimeOnly(1, 0) };
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = true, HorarioInicio = new TimeOnly(21, 0), HorarioFin = new TimeOnly(2, 0) }
            };

            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(restauranteId))
                .ReturnsAsync(new List<DOM.TurnoLaboral> { turnoActual });
            _turnoLaboralRepoMock
                .Setup(r => r.ActualizarTurnosLaboralesAsync(restauranteId, turnosEntrada))
                .ReturnsAsync(turnosEntrada);
            _calculadorVentanaTurnoServicioMock
                .Setup(s => s.CalcularVentana(turnoActual, It.IsAny<DateTime>()))
                .Throws<InvalidOperationException>();

            var resultado = await CrearCasoDeUso().EjecutarAsync(restauranteId, turnosEntrada);

            Assert.NotNull(resultado);
            _pagoRepoMock.Verify(
                r => r.ObtenerPagosParaCierreAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}
