using Moq;
using PanComido.Dominio.CasosDeUso.PlatoCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Platos
{
    public class EliminarPlatoCasoDeUsoTest
    {
        private readonly Mock<IPlatoRepositorio> _platoRepoMock;
        private readonly EliminarPlatoCasoDeUso _casoDeUso;

        public EliminarPlatoCasoDeUsoTest()
        {
            _platoRepoMock = new Mock<IPlatoRepositorio>();
            _casoDeUso = new EliminarPlatoCasoDeUso(_platoRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_LlamaAEliminarEnRepositorio()
        {
            // Preparar
            int platoId = 10;
            int restauranteId = 1;

            // Ejecutar
            await _casoDeUso.EjecutarAsync(platoId, restauranteId);

            // Verificar
            _platoRepoMock.Verify(r => r.EliminarAsync(platoId, restauranteId), Times.Once);
        }
    }
}
