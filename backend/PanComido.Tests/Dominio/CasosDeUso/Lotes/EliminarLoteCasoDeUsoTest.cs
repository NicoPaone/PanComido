using Moq;
using PanComido.Dominio.CasosDeUso.LoteCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Lotes
{
    public class EliminarLoteCasoDeUsoTest
    {
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly EliminarLoteCasoDeUso _casoDeUso;

        public EliminarLoteCasoDeUsoTest()
        {
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _casoDeUso = new EliminarLoteCasoDeUso(_loteRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElLoteNoTieneStock_LlamaAEliminarYRetornaTrue()
        {
            int restauranteId = 1;
            int loteId = 10;

            _loteRepoMock.Setup(r => r.ObtenerPorIdAsync(restauranteId, loteId))
                .ReturnsAsync(new Lote { Id = loteId, Cantidad = 0 });
            _loteRepoMock.Setup(r => r.EliminarAsync(restauranteId, loteId))
                .ReturnsAsync(true);

            bool resultado = await _casoDeUso.EjecutarAsync(restauranteId, loteId);

            Assert.True(resultado);
            _loteRepoMock.Verify(r => r.EliminarAsync(restauranteId, loteId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElLoteTieneStock_LanzaInvalidOperationException()
        {
            int restauranteId = 1;
            int loteId = 10;

            _loteRepoMock.Setup(r => r.ObtenerPorIdAsync(restauranteId, loteId))
                .ReturnsAsync(new Lote { Id = loteId, Cantidad = 5 });

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _casoDeUso.EjecutarAsync(restauranteId, loteId));

            _loteRepoMock.Verify(r => r.EliminarAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElLoteNoExiste_RetornaFalse()
        {
            int restauranteId = 1;
            int loteId = 10;

            _loteRepoMock.Setup(r => r.ObtenerPorIdAsync(restauranteId, loteId))
                .ReturnsAsync((Lote)null);

            bool resultado = await _casoDeUso.EjecutarAsync(restauranteId, loteId);

            Assert.False(resultado);
            _loteRepoMock.Verify(r => r.EliminarAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }
}
