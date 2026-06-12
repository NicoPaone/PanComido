using Moq;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Carta
{
    public class ModificarArticuloCasoDeUsoTest
    {
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly ModificarArticuloCasoDeUso _casoDeUso;

        public ModificarArticuloCasoDeUsoTest()
        {
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _casoDeUso = new ModificarArticuloCasoDeUso(_articuloRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloNoExiste_LanzaArgumentException()
        {
            // Preparar
            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(1, 10))
                             .ReturnsAsync((Articulo)null);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<System.ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(1, 10, true, true));

            Assert.Equal("El artículo que intenta modificar no existe o no pertenece al restaurante.", excepcion.Message);
            _articuloRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Articulo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloEsPlato_ActualizaVisibilidadYDestacado()
        {
            // Preparar
            var plato = new Plato { Id = 10, EsVisibleEnCarta = false, Destacado = false };
            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(1, 10))
                             .ReturnsAsync(plato);

            // Ejecutar
            await _casoDeUso.EjecutarAsync(1, 10, true, true);

            // Verificar
            Assert.True(plato.EsVisibleEnCarta);
            Assert.True(plato.Destacado);
            _articuloRepoMock.Verify(r => r.ActualizarAsync(plato), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloEsInsumo_SoloActualizaVisibilidad()
        {
            // Preparar
            var insumoBebida = new Insumo { Id = 5, EsVisibleEnCarta = false };
            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(1, 5))
                             .ReturnsAsync(insumoBebida);

            // Ejecutar
            // Mandamos true a destacado, pero al ser Insumo lo debe ignorar
            await _casoDeUso.EjecutarAsync(1, 5, true, true); 

            // Verificar
            Assert.True(insumoBebida.EsVisibleEnCarta);
            _articuloRepoMock.Verify(r => r.ActualizarAsync(insumoBebida), Times.Once);
        }
    }
}
