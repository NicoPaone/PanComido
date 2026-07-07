using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
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

            await _casoDeUso.EjecutarAsync(insumoId, restauranteId);

            _insumoRepoMock.Verify(r => r.EliminarAsync(insumoId, restauranteId), Times.Once);
        }
    }
}
