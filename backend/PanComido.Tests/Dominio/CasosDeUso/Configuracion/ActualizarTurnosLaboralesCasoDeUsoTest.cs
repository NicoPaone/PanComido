using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ActualizarTurnosLaboralesCasoDeUsoTest
    {
        private readonly Mock<ITurnoLaboralRepositorio> _turnoLaboralRepoMock;

        public ActualizarTurnosLaboralesCasoDeUsoTest()
        {
            _turnoLaboralRepoMock = new Mock<ITurnoLaboralRepositorio>();
        }

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

            var casoDeUso = new ActualizarTurnosLaboralesCasoDeUso(_turnoLaboralRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, turnosEntrada);

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

            var casoDeUso = new ActualizarTurnosLaboralesCasoDeUso(_turnoLaboralRepoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(restauranteId, turnosEntrada));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTurnoDiurnoConHorarioInicioIgualAlFin_LanzaArgumentException()
        {
            int restauranteId = 1;
            var turnosEntrada = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1, EsNocturno = false, HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(8, 0) }
            };

            var casoDeUso = new ActualizarTurnosLaboralesCasoDeUso(_turnoLaboralRepoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(restauranteId, turnosEntrada));
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

            var casoDeUso = new ActualizarTurnosLaboralesCasoDeUso(_turnoLaboralRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, turnosEntrada);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
        }
    }
}