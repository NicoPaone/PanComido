using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.BebidaPreparada
{
    public class CrearBebidaPreparadaCasoDeUsoTest
    {
        private readonly Mock<IBebidaPreparadaRepositorio> _bebidaPreparadaRepoMock;
        private readonly Mock<IInsumoValidacionServicio> _insumoValidacionServicioMock;
        private readonly Mock<IBebidaPreparadaValidacionServicio> _bebidaPreparadaValidacionServicioMock;
        private readonly Mock<IImagenServicio> _imagenServicioMock;
        private readonly CrearBebidaPreparadaCasoDeUso _casoDeUso;

        public CrearBebidaPreparadaCasoDeUsoTest()
        {
            _bebidaPreparadaRepoMock = new Mock<IBebidaPreparadaRepositorio>();
            _insumoValidacionServicioMock = new Mock<IInsumoValidacionServicio>();
            _bebidaPreparadaValidacionServicioMock = new Mock<IBebidaPreparadaValidacionServicio>();
            _imagenServicioMock = new Mock<IImagenServicio>();

            _casoDeUso = new CrearBebidaPreparadaCasoDeUso(
                _bebidaPreparadaRepoMock.Object,
                _insumoValidacionServicioMock.Object,
                _bebidaPreparadaValidacionServicioMock.Object,
                _imagenServicioMock.Object,
                Mock.Of<ILogger<CrearBebidaPreparadaCasoDeUso>>());
        }

        private static DOM.BebidaPreparada NuevaBebida() => new DOM.BebidaPreparada
        {
            Nombre = "Fernet con Coca",
            PrecioVentaFinal = 3000,
            Insumos = new List<DOM.BebidaPreparadaInsumo>
            {
                new DOM.BebidaPreparadaInsumo { InsumoId = 1, Cantidad = 100 },
                new DOM.BebidaPreparadaInsumo { InsumoId = 2, Cantidad = 300 }
            }
        };

        [Fact]
        public async Task EjecutarAsync_CuandoLaValidacionBasicaFalla_NoConsultaNombreNiCrea()
        {
            int restauranteId = 1;
            var bebida = NuevaBebida();

            _bebidaPreparadaValidacionServicioMock
                .Setup(s => s.ValidarDatosBasicos(bebida))
                .Throws(new ArgumentException("El nombre de la bebida preparada no puede estar vacío."));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", Stream.Null, ""));

            _bebidaPreparadaRepoMock.Verify(r => r.ExisteBebidaPreparadaConNombreAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _bebidaPreparadaRepoMock.Verify(r => r.CrearAsync(It.IsAny<DOM.BebidaPreparada>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNombreYaExiste_LanzaArgumentException()
        {
            int restauranteId = 1;
            var bebida = NuevaBebida();

            _bebidaPreparadaRepoMock.Setup(r => r.ExisteBebidaPreparadaConNombreAsync(restauranteId, bebida.Nombre))
                .ReturnsAsync(true);

            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", Stream.Null, ""));

            Assert.Equal($"Ya existe una bebida preparada con el nombre '{bebida.Nombre}' en el restaurante.", excepcion.Message);
            _insumoValidacionServicioMock.Verify(s => s.ValidarInsumosDeRecetaBebidaAsync(It.IsAny<int>(), It.IsAny<List<DOM.BebidaPreparadaInsumo>>()), Times.Never);
            _bebidaPreparadaRepoMock.Verify(r => r.CrearAsync(It.IsAny<DOM.BebidaPreparada>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaValidacionDeInsumosFalla_NoCreaLaBebida()
        {
            int restauranteId = 1;
            var bebida = NuevaBebida();

            _bebidaPreparadaRepoMock.Setup(r => r.ExisteBebidaPreparadaConNombreAsync(restauranteId, bebida.Nombre))
                .ReturnsAsync(false);
            _insumoValidacionServicioMock
                .Setup(s => s.ValidarInsumosDeRecetaBebidaAsync(restauranteId, bebida.Insumos))
                .ThrowsAsync(new ArgumentException("El insumo 'Tomate' no es de tipo Bebida y no puede usarse en la receta de una bebida preparada."));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", Stream.Null, ""));

            _bebidaPreparadaRepoMock.Verify(r => r.CrearAsync(It.IsAny<DOM.BebidaPreparada>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsValidoConImagen_SubeImagenYCrea()
        {
            int restauranteId = 1;
            var bebida = NuevaBebida();
            using Stream stream = new MemoryStream();

            _bebidaPreparadaRepoMock.Setup(r => r.ExisteBebidaPreparadaConNombreAsync(restauranteId, bebida.Nombre))
                .ReturnsAsync(false);
            _imagenServicioMock.Setup(s => s.SubirImagenAsync(stream, "foto.jpg", "carpeta"))
                .ReturnsAsync("url-nueva.jpg");
            _bebidaPreparadaRepoMock.Setup(r => r.CrearAsync(bebida))
                .ReturnsAsync(bebida);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", stream, "foto.jpg");

            Assert.Equal("url-nueva.jpg", bebida.UrlImagen);
            Assert.Equal(restauranteId, bebida.RestauranteId);
            Assert.Same(bebida, resultado);
            _bebidaPreparadaRepoMock.Verify(r => r.CrearAsync(bebida), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsValidoSinImagen_NoSubeImagenYCrea()
        {
            int restauranteId = 1;
            var bebida = NuevaBebida();

            _bebidaPreparadaRepoMock.Setup(r => r.ExisteBebidaPreparadaConNombreAsync(restauranteId, bebida.Nombre))
                .ReturnsAsync(false);
            _bebidaPreparadaRepoMock.Setup(r => r.CrearAsync(bebida))
                .ReturnsAsync(bebida);

            await _casoDeUso.EjecutarAsync(restauranteId, bebida, "carpeta", Stream.Null, "");

            Assert.Null(bebida.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _bebidaPreparadaRepoMock.Verify(r => r.CrearAsync(bebida), Times.Once);
        }
    }
}
