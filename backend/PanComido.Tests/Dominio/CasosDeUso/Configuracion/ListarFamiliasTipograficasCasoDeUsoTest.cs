using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ListarFamiliasTipograficasCasoDeUsoTest
    {
        private readonly Mock<IFamiliaTipograficaRepositorio> _familiaTipograficaRepoMock;

        public ListarFamiliasTipograficasCasoDeUsoTest()
        {
            _familiaTipograficaRepoMock = new Mock<IFamiliaTipograficaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayFamilias_DevuelveLista()
        {
            var familias = new List<DOM.FamiliaTipografica>
            {
                new DOM.FamiliaTipografica { Id = 1 },
                new DOM.FamiliaTipografica { Id = 2 }
            };

            _familiaTipograficaRepoMock
                .Setup(r => r.ListarTipografias())
                .ReturnsAsync(familias);

            var casoDeUso = new ListarFamiliasTipograficasCasoDeUso(_familiaTipograficaRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayFamilias_DevuelveListaVacia()
        {
            _familiaTipograficaRepoMock
                .Setup(r => r.ListarTipografias())
                .ReturnsAsync(new List<DOM.FamiliaTipografica>());

            var casoDeUso = new ListarFamiliasTipograficasCasoDeUso(_familiaTipograficaRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
    }
}