using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class DesasignarMozoMesaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;
        private readonly Mock<IMesaNotificador> _mesaNotificadorMock;

        public DesasignarMozoMesaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
            _mesaNotificadorMock = new Mock<IMesaNotificador>();
        }

        [Fact]
        public async Task EjecutarAsync_LlamaAlRepositorioConLosParametrosCorrectos()
        {
            int restauranteId = 1;
            int mesaId = 5;
            int mozoId = 2;

            var mesa = new MesaConPosiciones { Id = mesaId, MozosAsignadosIds = new List<int> { mozoId } };

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(mesa);

            _mesaMockRepo
                .Setup(r => r.DesasignarMozoAsync(mesaId, mozoId))
                .Returns(Task.CompletedTask);

            _mesaNotificadorMock
                .Setup(n => n.NotificarMesaActualizadaAsync(It.IsAny<MesaConPosiciones>(), restauranteId))
                .Returns(Task.CompletedTask);

            var casoDeUso = new DesasignarMozoMesaCasoDeUso(_mesaMockRepo.Object, _mesaNotificadorMock.Object);

            await casoDeUso.EjecutarAsync(restauranteId, mesaId, mozoId);

            _mesaMockRepo.Verify(r => r.DesasignarMozoAsync(mesaId, mozoId), Times.Once());
        }
    }
}
