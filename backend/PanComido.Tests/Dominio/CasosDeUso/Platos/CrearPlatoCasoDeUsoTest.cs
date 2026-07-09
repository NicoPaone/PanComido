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
    public class CrearPlatoCasoDeUsoTest
    {
        private readonly Mock<IPlatoRepositorio> _platoRepoMock;
        private readonly Mock<IInsumoValidacionServicio> _insumoValidacionServicioMock;
        private readonly Mock<IImagenServicio> _imagenServicioMock;
        private readonly Mock<INormalizadorNombreServicio> _normalizadorNombreServicioMock;
        private readonly CrearPlatoCasoDeUso _casoDeUso;

        public CrearPlatoCasoDeUsoTest()
        {
            _platoRepoMock = new Mock<IPlatoRepositorio>();
            _insumoValidacionServicioMock = new Mock<IInsumoValidacionServicio>();
            _imagenServicioMock = new Mock<IImagenServicio>();
            _normalizadorNombreServicioMock = new Mock<INormalizadorNombreServicio>();
            _normalizadorNombreServicioMock.Setup(s => s.Normalizar(It.IsAny<string>())).Returns((string nombre) => nombre);
            _casoDeUso = new CrearPlatoCasoDeUso(_platoRepoMock.Object, _insumoValidacionServicioMock.Object, _imagenServicioMock.Object, _normalizadorNombreServicioMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_NombreVacio_LanzaArgumentException()
        {
            // Preparar
            var plato = new Plato { Nombre = "", PrecioVentaFinal = 100 };
            
            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(1, plato, "cloudinary", Stream.Null, "imagen.png"));

            Assert.Equal("El nombre del plato no puede estar vacío.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_PrecioMenorOIgualACero_LanzaArgumentException()
        {
            // Preparar
            var plato = new Plato { Nombre = "Plato Test", PrecioVentaFinal = 0 };

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(1, plato, "cloudinary", Stream.Null, "imagen.png"));

            Assert.Equal("El precio de venta final debe ser mayor que cero.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_SinIngredientes_LanzaArgumentException()
        {
            // Preparar
            var plato = new Plato 
            { 
                Nombre = "Plato Test", 
                PrecioVentaFinal = 100, 
                Ingredientes = new List<PlatoIngrediente>() // Lista vacía
            };

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(1, plato, "cloudinary", Stream.Null, "imagen.png"));

            Assert.Equal("El plato debe tener al menos un ingrediente.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CantidadDeUnIngredienteEsCeroOMenor_LanzaArgumentException()
        {
            // Preparar
            var plato = new Plato
            {
                Nombre = "Plato Test",
                PrecioVentaFinal = 100,
                Ingredientes = new List<PlatoIngrediente> { new PlatoIngrediente { InsumoId = 1, Cantidad = 0 } }
            };

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, plato, "cloudinary", Stream.Null, "imagen.png"));

            Assert.Equal("La cantidad de cada ingrediente debe ser mayor que cero.", excepcion.Message);
            _platoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Plato>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_NombreYaExiste_LanzaArgumentException()
        {
            // Preparar
            var plato = new Plato 
            { 
                Nombre = "Plato Duplicado",
                PrecioVentaFinal = 100,
                Ingredientes = new List<PlatoIngrediente> { new PlatoIngrediente { Cantidad = 1 } }
            };

            _platoRepoMock.Setup(r => r.ExistePlatoConNombreAsync(1, "Plato Duplicado"))
                          .ReturnsAsync(true);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(1, plato, "cloudinary", Stream.Null, "imagen.png"));

            Assert.Equal($"Ya existe un plato con el nombre '{plato.Nombre}' en el restaurante.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_PlatoValidoConImagen_LlamaASubirImagenYCrear()
        {
            // Preparar
            var plato = new Plato 
            { 
                Nombre = "Plato OK",
                PrecioVentaFinal = 100,
                Ingredientes = new List<PlatoIngrediente> { new PlatoIngrediente { Cantidad = 1 } }
            };
            var stream = new MemoryStream();
            var urlImagenMock = "http://cloudinary.com/imagen.png";

            _platoRepoMock.Setup(r => r.ExistePlatoConNombreAsync(1, "Plato OK"))
                          .ReturnsAsync(false);

            _imagenServicioMock.Setup(s => s.SubirImagenAsync(stream, "imagen.png", "cloudinary"))
                               .ReturnsAsync(urlImagenMock);

            // Ejecutar
            await _casoDeUso.EjecutarAsync(1, plato, "cloudinary", stream, "imagen.png");

            // Verificar
            Assert.Equal(urlImagenMock, plato.UrlImagen);
            Assert.Equal(1, plato.RestauranteId);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(stream, "imagen.png", "cloudinary"), Times.Once);
            _platoRepoMock.Verify(r => r.CrearAsync(plato), Times.Once);
        }
    }
}
