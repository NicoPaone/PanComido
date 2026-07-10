using Moq;
using PanComido.Dominio.CasosDeUso.PlatoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Platos
{
    public class ModificarPlatoCasoDeUsoTest
    {
        private readonly Mock<IPlatoRepositorio> _platoRepoMock;
        private readonly Mock<IInsumoValidacionServicio> _insumoValidacionServicioMock;
        private readonly Mock<IImagenServicio> _imagenServicioMock;
        private readonly Mock<INormalizadorNombreServicio> _normalizadorNombreServicioMock;
        private readonly ModificarPlatoCasoDeUso _casoDeUso;

        public ModificarPlatoCasoDeUsoTest()
        {
            _platoRepoMock = new Mock<IPlatoRepositorio>();
            _insumoValidacionServicioMock = new Mock<IInsumoValidacionServicio>();
            _imagenServicioMock = new Mock<IImagenServicio>();
            _normalizadorNombreServicioMock = new Mock<INormalizadorNombreServicio>();
            _normalizadorNombreServicioMock.Setup(s => s.Normalizar(It.IsAny<string>())).Returns((string nombre) => nombre);
            _casoDeUso = new ModificarPlatoCasoDeUso(_platoRepoMock.Object, _insumoValidacionServicioMock.Object, _imagenServicioMock.Object, _normalizadorNombreServicioMock.Object);
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
        public async Task EjecutarAsync_CuandoElNombreEsVacio_LanzaArgumentException()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "" };
            var platoExistenteDb = new Plato { Id = 10, Nombre = "Plato Viejo" };

            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync(platoExistenteDb);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, platoModificado, "", Stream.Null, ""));

            Assert.Equal("El nombre del plato no puede estar vacío.", excepcion.Message);
            _platoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Plato>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNuevoNombreYaLoUsaOtroPlato_LanzaArgumentException()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "Milanesa" };
            var platoExistenteDb = new Plato { Id = 10, Nombre = "Plato Viejo" };

            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync(platoExistenteDb);
            _platoRepoMock.Setup(r => r.ExistePlatoConNombreAsync(restauranteId, platoModificado.Nombre))
                          .ReturnsAsync(true);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, platoModificado, "", Stream.Null, ""));

            Assert.Equal($"Ya existe un plato con el nombre '{platoModificado.Nombre}' en el restaurante.", excepcion.Message);
            _platoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Plato>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNombreNoCambia_NoConsultaDuplicados()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "Plato Viejo", PrecioVentaFinal = 500 };
            var platoExistenteDb = new Plato { Id = 10, Nombre = "Plato Viejo", PrecioVentaFinal = 200 };

            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync(platoExistenteDb);

            // Ejecutar
            await _casoDeUso.EjecutarAsync(restauranteId, platoModificado, "", Stream.Null, "");

            // Verificar
            _platoRepoMock.Verify(r => r.ExistePlatoConNombreAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _platoRepoMock.Verify(r => r.ActualizarAsync(platoExistenteDb), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CantidadDeUnIngredienteEsCeroOMenor_LanzaArgumentException()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato
            {
                Id = 10,
                Nombre = "Plato Editado",
                Ingredientes = new List<PlatoIngrediente> { new PlatoIngrediente { InsumoId = 1, Cantidad = -1 } }
            };
            var platoExistenteDb = new Plato { Id = 10, Nombre = "Plato Viejo" };

            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync(platoExistenteDb);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, platoModificado, "", Stream.Null, ""));

            Assert.Equal("La cantidad de cada ingrediente debe ser mayor que cero.", excepcion.Message);
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
