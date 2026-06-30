using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class AsignarMozosMesaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;
        private readonly Mock<IMesaNotificador> _mesaNotificadorMock;

        public AsignarMozosMesaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
            _mesaNotificadorMock = new Mock<IMesaNotificador>();
        }

        [Fact]
        public async Task EjecutarAsync_LlamaAlRepositorioConElDiffCorrecto()
        {
            int restauranteId = 1;
            int mesaId = 5;
            var mozosActuales = new List<int> { 1, 2 };
            var mozosNuevos = new List<int> { 2, 3 };

            var mesa = new MesaConPosiciones { Id = mesaId, MozosAsignadosIds = mozosActuales };

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(mesa);

            _mesaMockRepo
                .Setup(r => r.AsignarMozosAsync(mesaId, It.IsAny<List<int>>(), It.IsAny<List<int>>()))
                .Returns(Task.CompletedTask);

            _mesaNotificadorMock
                .Setup(n => n.NotificarMesaActualizadaAsync(It.IsAny<MesaConPosiciones>(), restauranteId))
                .Returns(Task.CompletedTask);

            var casoDeUso = new AsignarMozosMesaCasoDeUso(_mesaMockRepo.Object, _mesaNotificadorMock.Object);

            await casoDeUso.EjecutarAsync(restauranteId, mesaId, mozosNuevos);

            _mesaMockRepo.Verify(r => r.AsignarMozosAsync(
                mesaId,
                It.Is<List<int>>(l => l.SequenceEqual(new List<int> { 3 })),
                It.Is<List<int>>(l => l.SequenceEqual(new List<int> { 1 }))),
                Times.Once());
        }
    }
}
