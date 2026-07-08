using Moq;
using PanComido.Dominio.CasosDeUso.PlatoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Platos
{
    public class ModificarPlatoCasoDeUsoTest
    {
        private readonly Mock<IPlatoRepositorio> _platoRepoMock;
        private readonly Mock<IImagenServicio> _imagenServicioMock;
        private readonly ModificarPlatoCasoDeUso _casoDeUso;

        public ModificarPlatoCasoDeUsoTest()
        {
            _platoRepoMock = new Mock<IPlatoRepositorio>();
            _imagenServicioMock = new Mock<IImagenServicio>();
            _casoDeUso = new ModificarPlatoCasoDeUso(_platoRepoMock.Object, _imagenServicioMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoPlatoNoExiste_LanzaArgumentException()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "Nuevo Nombre" };

            // Simulamos que buscar el plato en la BD devuelve nulo
            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync((Plato)null);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, platoModificado, "", Stream.Null, ""));

            Assert.Equal("El plato que intenta modificar no existe o no pertenece al restaurante.", excepcion.Message);

            // Verificamos que el repositorio nunca intentó guardar en BD
            _platoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Plato>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoPlatoExiste_ActualizaPropiedadesYLlamaActualizarAsync()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "Plato Editado", PrecioVentaFinal = 500, EsPrecioManual = true };
            var platoExistenteDb = new Plato { Id = 10, Nombre = "Plato Viejo", PrecioVentaFinal = 200, EsPrecioManual = false };

            // Simulamos que el plato existe en BD
            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync(platoExistenteDb);

            // Ejecutar (sin imagen nueva)
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, platoModificado, "", Stream.Null, "");

            // Verificar
            Assert.Equal("Plato Editado", platoExistenteDb.Nombre);
            Assert.Equal(500, platoExistenteDb.PrecioVentaFinal);
            Assert.True(platoExistenteDb.EsPrecioManual);
            Assert.Same(platoExistenteDb, resultado);

            // Verificamos que se llamó al método de guardar exactamente una vez
            _platoRepoMock.Verify(r => r.ActualizarAsync(platoExistenteDb), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoLlegaImagenNueva_ConservaLaUrlImagenExistente()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "Plato Editado" };
            var platoExistenteDb = new Plato { Id = 10, Nombre = "Plato Viejo", UrlImagen = "https://cloudinary/vieja.jpg" };

            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync(platoExistenteDb);

            // Ejecutar (sin stream ni nombre de imagen)
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, platoModificado, "carpeta", Stream.Null, "");

            // Verificar: la url vieja se mantiene intacta
            Assert.Equal("https://cloudinary/vieja.jpg", resultado.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLlegaImagenNueva_LaSubeYReemplazaLaUrlImagen()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "Plato Editado" };
            var platoExistenteDb = new Plato { Id = 10, Nombre = "Plato Viejo", UrlImagen = "https://cloudinary/vieja.jpg" };

            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync(platoExistenteDb);
            _imagenServicioMock.Setup(s => s.SubirImagenAsync(It.IsAny<Stream>(), "nueva.jpg", "carpeta"))
                          .ReturnsAsync("https://cloudinary/nueva.jpg");

            // Ejecutar (con stream y nombre de imagen)
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, platoModificado, "carpeta", Stream.Null, "nueva.jpg");

            // Verificar: la url se reemplaza por la nueva
            Assert.Equal("https://cloudinary/nueva.jpg", resultado.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(It.IsAny<Stream>(), "nueva.jpg", "carpeta"), Times.Once);
        }
    }
}
