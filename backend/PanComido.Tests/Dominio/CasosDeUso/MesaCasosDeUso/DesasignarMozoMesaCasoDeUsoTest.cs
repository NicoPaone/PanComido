using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class DesasignarMozoMesaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;

        public DesasignarMozoMesaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_LlamaAlRepositorioConLosParametrosCorrectos()
        {
            int restauranteId = 1;
            int mesaId = 5;
            int mozoId = 2;

            _mesaMockRepo
                .Setup(r => r.DesasignarMozoAsync(restauranteId, mesaId, mozoId))
                .Returns(Task.CompletedTask);

            var casoDeUso = new DesasignarMozoMesaCasoDeUso(_mesaMockRepo.Object);

            await casoDeUso.EjecutarAsync(restauranteId, mesaId, mozoId);

            _mesaMockRepo.Verify(r => r.DesasignarMozoAsync(restauranteId, mesaId, mozoId), Times.Once());
        }
    }
}
