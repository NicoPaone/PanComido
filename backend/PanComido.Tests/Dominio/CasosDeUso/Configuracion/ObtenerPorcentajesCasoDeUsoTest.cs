using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ObtenerPorcentajesCasoDeUsoTest
    {
        private readonly Mock<IPorcentajesCategoriaRepositorio> _porcentajesRepoMock;

        public ObtenerPorcentajesCasoDeUsoTest()
        {
            _porcentajesRepoMock = new Mock<IPorcentajesCategoriaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExistenPorcentajes_DevuelvePorcentajesGanancia()
        {
            int restauranteId = 1;
            var porcentajes = new DOM.PorcentajesGanancia();

            _porcentajesRepoMock
                .Setup(r => r.ObtenerPorcentajesGananciaAsync(restauranteId))
                .ReturnsAsync(porcentajes);

            var casoDeUso = new ObtenerPorcentajesCasoDeUso(_porcentajesRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
        }
    }
}