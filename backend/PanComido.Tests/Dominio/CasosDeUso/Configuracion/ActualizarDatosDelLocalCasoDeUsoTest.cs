using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ActualizarDatosDelLocalCasoDeUsoTest
    {
        private readonly Mock<IRestauranteRepositorio> _restauranteRepoMock;
        private readonly Mock<IImagenServicio> _imagenServicioMock;
        private readonly Mock<ILogger<ActualizarDatosDelLocalCasoDeUso>> _loggerMock;

        public ActualizarDatosDelLocalCasoDeUsoTest()
        {
            _restauranteRepoMock = new Mock<IRestauranteRepositorio>();
            _imagenServicioMock = new Mock<IImagenServicio>();
            _loggerMock = new Mock<ILogger<ActualizarDatosDelLocalCasoDeUso>>();
        }

        private ActualizarDatosDelLocalCasoDeUso CrearCasoDeUso() =>
            new ActualizarDatosDelLocalCasoDeUso(
                _restauranteRepoMock.Object,
                _imagenServicioMock.Object,
                _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_SinImagen_NoSubeImagenYDevuelveRestaurante()
        {
            int restauranteId = 1;
            var restauranteDatos = new DOM.Restaurante { Id = restauranteId };
            var restauranteActualizado = new DOM.Restaurante { Id = restauranteId, Nombre = "Pan Comido" };

            _restauranteRepoMock
                .Setup(r => r.ActualizarDatosDelLocalAsync(restauranteId, It.IsAny<DOM.Restaurante>()))
                .Returns(Task.CompletedTask);

            _restauranteRepoMock
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(restauranteActualizado);

            var resultado = await CrearCasoDeUso().EjecutarAsync(
                restauranteId, restauranteDatos, "carpeta", null, null);

            Assert.NotNull(resultado);
            _imagenServicioMock.Verify(
                s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_ConImagen_SubeImagenYActualiza()
        {
            int restauranteId = 1;
            var restauranteDatos = new DOM.Restaurante { Id = restauranteId };
            var urlImagen = "https://cloudinary.com/imagen.jpg";
            var restauranteActualizado = new DOM.Restaurante { Id = restauranteId, Imagen = urlImagen };
            using var stream = new MemoryStream();

            _imagenServicioMock
                .Setup(s => s.SubirImagenAsync(stream, "imagen.jpg", "carpeta"))
                .ReturnsAsync(urlImagen);

            _restauranteRepoMock
                .Setup(r => r.ActualizarDatosDelLocalAsync(restauranteId, It.IsAny<DOM.Restaurante>()))
                .Returns(Task.CompletedTask);

            _restauranteRepoMock
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(restauranteActualizado);

            var resultado = await CrearCasoDeUso().EjecutarAsync(
                restauranteId, restauranteDatos, "carpeta", stream, "imagen.jpg");

            Assert.NotNull(resultado);
            Assert.Equal(urlImagen, resultado.Imagen);
            _imagenServicioMock.Verify(
                s => s.SubirImagenAsync(stream, "imagen.jpg", "carpeta"),
                Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_ConStreamPeroSinNombreImagen_NoSubeImagenYActualiza()
        {
            int restauranteId = 1;
            var restauranteDatos = new DOM.Restaurante { Id = restauranteId };
            var restauranteActualizado = new DOM.Restaurante { Id = restauranteId, Nombre = "Pan Comido" };
            using var stream = new MemoryStream();

            _restauranteRepoMock
                .Setup(r => r.ActualizarDatosDelLocalAsync(restauranteId, It.IsAny<DOM.Restaurante>()))
                .Returns(Task.CompletedTask);

            _restauranteRepoMock
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(restauranteActualizado);

            var resultado = await CrearCasoDeUso().EjecutarAsync(
                restauranteId, restauranteDatos, "carpeta", stream, null);

            Assert.NotNull(resultado);
            _imagenServicioMock.Verify(
                s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}