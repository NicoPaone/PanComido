using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.BebidaPreparada
{
    public class ModificarBebidaPreparadaCasoDeUsoTest
    {
        private readonly Mock<IBebidaPreparadaRepositorio> _bebidaPreparadaRepoMock;
        private readonly Mock<IInsumoValidacionServicio> _insumoValidacionServicioMock;
        private readonly Mock<IBebidaPreparadaValidacionServicio> _bebidaPreparadaValidacionServicioMock;
        private readonly Mock<IImagenServicio> _imagenServicioMock;
        private readonly ModificarBebidaPreparadaCasoDeUso _casoDeUso;

        public ModificarBebidaPreparadaCasoDeUsoTest()
        {
            _bebidaPreparadaRepoMock = new Mock<IBebidaPreparadaRepositorio>();
            _insumoValidacionServicioMock = new Mock<IInsumoValidacionServicio>();
            _bebidaPreparadaValidacionServicioMock = new Mock<IBebidaPreparadaValidacionServicio>();
            _imagenServicioMock = new Mock<IImagenServicio>();

            _casoDeUso = new ModificarBebidaPreparadaCasoDeUso(
                _bebidaPreparadaRepoMock.Object,
                _insumoValidacionServicioMock.Object,
                _bebidaPreparadaValidacionServicioMock.Object,
                _imagenServicioMock.Object,
                Mock.Of<ILogger<ModificarBebidaPreparadaCasoDeUso>>());
        }

        private static DOM.BebidaPreparada BebidaModificada() => new DOM.BebidaPreparada
        {
            Id = 10,
            Nombre = "Fernet con Coca",
            PrecioVentaFinal = 3500,
            Insumos = new List<DOM.BebidaPreparadaInsumo>
            {
                new DOM.BebidaPreparadaInsumo { InsumoId = 1, Cantidad = 100 }
            }
        };

        [Fact]
        public async Task EjecutarAsync_CuandoNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            var bebida = BebidaModificada();

            _bebidaPreparadaRepoMock.Setup(r => r.ObtenerPorIdAsync(bebida.Id, restauranteId))
                .ReturnsAsync((DOM.BebidaPreparada)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", Stream.Null, ""));

            _insumoValidacionServicioMock.Verify(s => s.ValidarInsumosDeRecetaBebidaAsync(It.IsAny<int>(), It.IsAny<List<DOM.BebidaPreparadaInsumo>>()), Times.Never);
            _bebidaPreparadaRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<DOM.BebidaPreparada>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_SinImagenNueva_ConservaUrlImagenExistente()
        {
            int restauranteId = 1;
            var bebida = BebidaModificada();
            var bebidaExistente = new DOM.BebidaPreparada { Id = 10, UrlImagen = "url-existente.jpg" };

            _bebidaPreparadaRepoMock.Setup(r => r.ObtenerPorIdAsync(bebida.Id, restauranteId))
                .ReturnsAsync(bebidaExistente);
            _bebidaPreparadaRepoMock.Setup(r => r.ActualizarAsync(bebida))
                .ReturnsAsync(bebida);

            await _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", Stream.Null, "");

            Assert.Equal("url-existente.jpg", bebida.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_ConImagenNueva_SubeImagenYActualizaUrl()
        {
            int restauranteId = 1;
            var bebida = BebidaModificada();
            var bebidaExistente = new DOM.BebidaPreparada { Id = 10, UrlImagen = "url-vieja.jpg" };
            using Stream stream = new MemoryStream();

            _bebidaPreparadaRepoMock.Setup(r => r.ObtenerPorIdAsync(bebida.Id, restauranteId))
                .ReturnsAsync(bebidaExistente);
            _imagenServicioMock.Setup(s => s.SubirImagenAsync(stream, "foto.jpg", "carpeta"))
                .ReturnsAsync("url-nueva.jpg");
            _bebidaPreparadaRepoMock.Setup(r => r.ActualizarAsync(bebida))
                .ReturnsAsync(bebida);

            await _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", stream, "foto.jpg");

            Assert.Equal("url-nueva.jpg", bebida.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(stream, "foto.jpg", "carpeta"), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsValido_ActualizaYDevuelveLaBebida()
        {
            int restauranteId = 1;
            var bebida = BebidaModificada();
            var bebidaExistente = new DOM.BebidaPreparada { Id = 10, UrlImagen = "url.jpg" };
            var bebidaActualizada = BebidaModificada();

            _bebidaPreparadaRepoMock.Setup(r => r.ObtenerPorIdAsync(bebida.Id, restauranteId))
                .ReturnsAsync(bebidaExistente);
            _bebidaPreparadaRepoMock.Setup(r => r.ActualizarAsync(bebida))
                .ReturnsAsync(bebidaActualizada);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", Stream.Null, "");

            Assert.Equal(restauranteId, bebida.RestauranteId);
            Assert.Same(bebidaActualizada, resultado);
            _bebidaPreparadaRepoMock.Verify(r => r.ActualizarAsync(bebida), Times.Once);
        }
    }
}
