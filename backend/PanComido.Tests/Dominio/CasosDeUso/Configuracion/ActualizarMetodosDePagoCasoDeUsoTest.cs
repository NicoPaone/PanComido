using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ActualizarMetodosDePagoCasoDeUsoTest
    {
        private readonly Mock<IMetodoDePagoRepositorio> _metodoDePagoRepoMock;

        public ActualizarMetodosDePagoCasoDeUsoTest()
        {
            _metodoDePagoRepoMock = new Mock<IMetodoDePagoRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoActualiza_LlamaAlRepositorio()
        {
            int restauranteId = 1;
            var metodos = new List<DOM.MetodoDePago>
            {
                new DOM.MetodoDePago { Id = 1 },
                new DOM.MetodoDePago { Id = 2 }
            };

            _metodoDePagoRepoMock
                .Setup(r => r.ActualizarEstadoAsync(restauranteId, metodos))
                .Returns(Task.CompletedTask);

            var casoDeUso = new ActualizarMetodosDePagoCasoDeUso(_metodoDePagoRepoMock.Object);

            await casoDeUso.EjecutarAsync(restauranteId, metodos);

            _metodoDePagoRepoMock.Verify(r => r.ActualizarEstadoAsync(restauranteId, metodos), Times.Once);
        }
    }
}