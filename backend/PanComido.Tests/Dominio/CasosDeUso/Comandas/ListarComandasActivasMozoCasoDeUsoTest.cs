using Moq;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Comandas
{
    public class ListarComandasActivasMozoCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaRepoMock;

        public ListarComandasActivasMozoCasoDeUsoTest()
        {
            _comandaRepoMock = new Mock<IComandaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayComandasActivas_DevuelveLista()
        {
            int restauranteId = 1;
            int mozoId = 1;

            var comandas = new List<DOM.Comanda>
            {
                new DOM.Comanda { Id = 1, MesaId = 1, RestauranteId = restauranteId },
                new DOM.Comanda { Id = 2, MesaId = 2, RestauranteId = restauranteId }
            };

            _comandaRepoMock
                .Setup(r => r.ObtenerComandasActivasPorMozoAsync(restauranteId, mozoId))
                .ReturnsAsync(comandas);

            var casoDeUso = new ListarComandasActivasMozoCasoDeUso(
            _comandaRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, mozoId);
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }
    }
}
