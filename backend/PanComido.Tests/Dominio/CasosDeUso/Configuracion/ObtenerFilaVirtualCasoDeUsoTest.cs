using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ObtenerFilaVirtualCasoDeUsoTest
    {
        private readonly Mock<IFilaVirtualRepositorio> _filaVirtualRepoMock;

        public ObtenerFilaVirtualCasoDeUsoTest()
        {
            _filaVirtualRepoMock = new Mock<IFilaVirtualRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExisteFilaVirtual_DevuelveFilaVirtual()
        {
            int restauranteId = 1;
            var filaVirtual = new DOM.FilaVirtual { Id = 1, RestauranteId = restauranteId };

            _filaVirtualRepoMock
                .Setup(r => r.ObtenerFilaVirtualAsync(restauranteId))
                .ReturnsAsync(filaVirtual);

            var casoDeUso = new ObtenerFilaVirtualCasoDeUso(_filaVirtualRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(filaVirtual.Id, resultado.Id);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoExisteFilaVirtual_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;

            _filaVirtualRepoMock
                .Setup(r => r.ObtenerFilaVirtualAsync(restauranteId))
                .ReturnsAsync((DOM.FilaVirtual?)null);

            var casoDeUso = new ObtenerFilaVirtualCasoDeUso(_filaVirtualRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(restauranteId));
        }
    }
}