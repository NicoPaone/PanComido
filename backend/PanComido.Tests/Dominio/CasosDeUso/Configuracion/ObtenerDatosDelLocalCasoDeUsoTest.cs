using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ObtenerDatosDelLocalCasoDeUsoTest
    {
        private readonly Mock<IRestauranteRepositorio> _restauranteRepoMock;

        public ObtenerDatosDelLocalCasoDeUsoTest()
        {
            _restauranteRepoMock = new Mock<IRestauranteRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExisteRestaurante_DevuelveRestaurante()
        {
            int restauranteId = 1;
            var restaurante = new DOM.Restaurante { Id = restauranteId, Nombre = "Pan Comido" };

            _restauranteRepoMock
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(restaurante);

            var casoDeUso = new ObtenerDatosDelLocalCasoDeUso(_restauranteRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(restauranteId, resultado.Id);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoExisteRestaurante_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;

            _restauranteRepoMock
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync((DOM.Restaurante?)null);

            var casoDeUso = new ObtenerDatosDelLocalCasoDeUso(_restauranteRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(restauranteId));
        }
    }
}