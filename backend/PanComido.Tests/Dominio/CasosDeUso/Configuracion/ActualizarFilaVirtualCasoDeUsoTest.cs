using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ActualizarFilaVirtualCasoDeUsoTest
    {
        private readonly Mock<IFilaVirtualRepositorio> _filaVirtualRepoMock;

        public ActualizarFilaVirtualCasoDeUsoTest()
        {
            _filaVirtualRepoMock = new Mock<IFilaVirtualRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoSeHabilita_DevuelveFilaVirtualHabilitada()
        {
            int restauranteId = 1;
            bool habilitada = true;
            var filaActualizada = new DOM.FilaVirtual { Id = 1, RestauranteId = restauranteId, Habilitada = true };

            _filaVirtualRepoMock
                .Setup(r => r.ActualizarFilaVirtualAsync(restauranteId, habilitada))
                .ReturnsAsync(filaActualizada);

            var casoDeUso = new ActualizarFilaVirtualCasoDeUso(_filaVirtualRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, habilitada);

            Assert.NotNull(resultado);
            Assert.True(resultado.Habilitada);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoSeDeshabilita_DevuelveFilaVirtualDeshabilitada()
        {
            int restauranteId = 1;
            bool habilitada = false;
            var filaActualizada = new DOM.FilaVirtual { Id = 1, RestauranteId = restauranteId, Habilitada = false };

            _filaVirtualRepoMock
                .Setup(r => r.ActualizarFilaVirtualAsync(restauranteId, habilitada))
                .ReturnsAsync(filaActualizada);

            var casoDeUso = new ActualizarFilaVirtualCasoDeUso(_filaVirtualRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, habilitada);

            Assert.NotNull(resultado);
            Assert.False(resultado.Habilitada);
        }
    }
}