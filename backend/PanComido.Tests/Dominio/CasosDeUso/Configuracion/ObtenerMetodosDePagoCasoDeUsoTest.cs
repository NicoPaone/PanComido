using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ObtenerMetodosDePagoCasoDeUsoTest
    {
        private readonly Mock<IMetodoDePagoRepositorio> _metodoDePagoRepoMock;

        public ObtenerMetodosDePagoCasoDeUsoTest()
        {
            _metodoDePagoRepoMock = new Mock<IMetodoDePagoRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayMetodos_DevuelveLista()
        {
            int restauranteId = 1;
            var metodos = new List<DOM.MetodoDePago>
            {
                new DOM.MetodoDePago { Id = 1 },
                new DOM.MetodoDePago { Id = 2 }
            };

            _metodoDePagoRepoMock
                .Setup(r => r.ObtenerMetodosDePagoAsync(restauranteId))
                .ReturnsAsync(metodos);

            var casoDeUso = new ObtenerMetodosDePagoCasoDeUso(_metodoDePagoRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayMetodos_DevuelveListaVacia()
        {
            int restauranteId = 1;

            _metodoDePagoRepoMock
                .Setup(r => r.ObtenerMetodosDePagoAsync(restauranteId))
                .ReturnsAsync(new List<DOM.MetodoDePago>());

            var casoDeUso = new ObtenerMetodosDePagoCasoDeUso(_metodoDePagoRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
    }
}