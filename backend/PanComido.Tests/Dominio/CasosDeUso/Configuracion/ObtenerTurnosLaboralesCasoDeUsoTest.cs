using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ObtenerTurnosLaboralesCasoDeUsoTest
    {
        private readonly Mock<ITurnoLaboralRepositorio> _turnoLaboralRepoMock;

        public ObtenerTurnosLaboralesCasoDeUsoTest()
        {
            _turnoLaboralRepoMock = new Mock<ITurnoLaboralRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayTurnos_DevuelveLista()
        {
            int restauranteId = 1;
            var turnos = new List<DOM.TurnoLaboral>
            {
                new DOM.TurnoLaboral { Id = 1 },
                new DOM.TurnoLaboral { Id = 2 }
            };

            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(restauranteId))
                .ReturnsAsync(turnos);

            var casoDeUso = new ObtenerTurnosLaboralesCasoDeUso(_turnoLaboralRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayTurnos_DevuelveListaVacia()
        {
            int restauranteId = 1;

            _turnoLaboralRepoMock
                .Setup(r => r.ObtenerTurnosLaboralesAsync(restauranteId))
                .ReturnsAsync(new List<DOM.TurnoLaboral>());

            var casoDeUso = new ObtenerTurnosLaboralesCasoDeUso(_turnoLaboralRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
    }
}