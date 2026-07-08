using Moq;
using PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.BebidaPreparada
{
    public class EliminarBebidaPreparadaCasoDeUsoTest
    {
        private readonly Mock<IBebidaPreparadaRepositorio> _bebidaPreparadaRepoMock;
        private readonly EliminarBebidaPreparadaCasoDeUso _casoDeUso;

        public EliminarBebidaPreparadaCasoDeUsoTest()
        {
            _bebidaPreparadaRepoMock = new Mock<IBebidaPreparadaRepositorio>();
            _casoDeUso = new EliminarBebidaPreparadaCasoDeUso(_bebidaPreparadaRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_LlamaAEliminarEnRepositorio()
        {
            int bebidaPreparadaId = 10;
            int restauranteId = 1;

            _bebidaPreparadaRepoMock.Setup(r => r.EliminarAsync(bebidaPreparadaId, restauranteId))
                .ReturnsAsync(new DOM.BebidaPreparada { Id = bebidaPreparadaId, Nombre = "Fernet con Coca" });

            await _casoDeUso.EjecutarAsync(bebidaPreparadaId, restauranteId);

            _bebidaPreparadaRepoMock.Verify(r => r.EliminarAsync(bebidaPreparadaId, restauranteId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoExiste_LanzaKeyNotFoundException()
        {
            int bebidaPreparadaId = 10;
            int restauranteId = 1;

            _bebidaPreparadaRepoMock.Setup(r => r.EliminarAsync(bebidaPreparadaId, restauranteId))
                .ReturnsAsync((DOM.BebidaPreparada)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _casoDeUso.EjecutarAsync(bebidaPreparadaId, restauranteId));
        }
    }
}
