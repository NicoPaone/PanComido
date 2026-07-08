using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class EliminarInsumoCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly EliminarInsumoCasoDeUso _casoDeUso;

        public EliminarInsumoCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _casoDeUso = new EliminarInsumoCasoDeUso(_insumoRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_LlamaAEliminarEnRepositorio()
        {
            int insumoId = 10;
            int restauranteId = 1;

            _insumoRepoMock.Setup(r => r.EliminarAsync(insumoId, restauranteId))
                .ReturnsAsync(new Insumo { Id = insumoId, Nombre = "Tomate" });

            await _casoDeUso.EjecutarAsync(insumoId, restauranteId);

            _insumoRepoMock.Verify(r => r.EliminarAsync(insumoId, restauranteId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoExiste_LanzaKeyNotFoundException()
        {
            int insumoId = 10;
            int restauranteId = 1;

            _insumoRepoMock.Setup(r => r.EliminarAsync(insumoId, restauranteId))
                .ReturnsAsync((Insumo)null);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(() =>
                _casoDeUso.EjecutarAsync(insumoId, restauranteId));
        }
    }
}
