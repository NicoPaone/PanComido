using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class AsignarMozosMesaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;

        public AsignarMozosMesaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_LlamaAlRepositorioConLosParametrosCorrectos()
        {
            int restauranteId = 1;
            int mesaId = 5;
            var mozosIds = new List<int> { 1, 2 };

            _mesaMockRepo
                .Setup(r => r.AsignarMozosAsync(restauranteId, mesaId, mozosIds))
                .Returns(Task.CompletedTask);

            var casoDeUso = new AsignarMozosMesaCasoDeUso(_mesaMockRepo.Object);

            await casoDeUso.EjecutarAsync(restauranteId, mesaId, mozosIds);

            _mesaMockRepo.Verify(r => r.AsignarMozosAsync(restauranteId, mesaId, mozosIds), Times.Once());
        }
    }
}
