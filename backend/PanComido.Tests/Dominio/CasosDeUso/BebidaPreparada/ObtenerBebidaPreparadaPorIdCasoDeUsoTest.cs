using Moq;
using PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.BebidaPreparada
{
    public class ObtenerBebidaPreparadaPorIdCasoDeUsoTest
    {
        private readonly Mock<IBebidaPreparadaRepositorio> _bebidaPreparadaRepoMock;
        private readonly ObtenerBebidaPreparadaPorIdCasoDeUso _casoDeUso;

        public ObtenerBebidaPreparadaPorIdCasoDeUsoTest()
        {
            _bebidaPreparadaRepoMock = new Mock<IBebidaPreparadaRepositorio>();
            _casoDeUso = new ObtenerBebidaPreparadaPorIdCasoDeUso(_bebidaPreparadaRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExiste_DevuelveLaBebidaPreparada()
        {
            int bebidaPreparadaId = 10;
            int restauranteId = 1;
            var bebida = new DOM.BebidaPreparada { Id = bebidaPreparadaId, Nombre = "Fernet con Coca" };

            _bebidaPreparadaRepoMock.Setup(r => r.ObtenerPorIdAsync(bebidaPreparadaId, restauranteId))
                .ReturnsAsync(bebida);

            var resultado = await _casoDeUso.EjecutarAsync(bebidaPreparadaId, restauranteId);

            Assert.Same(bebida, resultado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoExiste_LanzaKeyNotFoundException()
        {
            int bebidaPreparadaId = 10;
            int restauranteId = 1;

            _bebidaPreparadaRepoMock.Setup(r => r.ObtenerPorIdAsync(bebidaPreparadaId, restauranteId))
                .ReturnsAsync((DOM.BebidaPreparada)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _casoDeUso.EjecutarAsync(bebidaPreparadaId, restauranteId));
        }
    }
}
