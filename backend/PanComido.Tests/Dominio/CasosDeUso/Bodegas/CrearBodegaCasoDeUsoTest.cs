using Moq;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Bodegas
{
    public class CrearBodegaCasoDeUsoTest
    {
        private readonly Mock<IBodegaRepositorio> _bodegaRepositorioMock;
        private readonly Mock<ITipoBodegaRepositorio> _tipoBodegaRepositorioMock;
        private readonly CrearBodegaCasoDeUso _casoDeUso;

        public CrearBodegaCasoDeUsoTest()
        {
            _bodegaRepositorioMock = new Mock<IBodegaRepositorio>();
            _tipoBodegaRepositorioMock = new Mock<ITipoBodegaRepositorio>();
            _casoDeUso = new CrearBodegaCasoDeUso(_bodegaRepositorioMock.Object, _tipoBodegaRepositorioMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_NombreValido_LlamaRepositorioYRetornaBodega()
        {
            // Preparar
            var restauranteId = 1;
            var bodega = new Bodega { Nombre = "Bodega Central", TipoBodegaId = 2 };
            var bodegaCreada = new Bodega { Id = 10, Nombre = "Bodega Central", TipoBodegaId = 2 };

            _tipoBodegaRepositorioMock
                .Setup(r => r.ExisteAsync(bodega.TipoBodegaId))
                .ReturnsAsync(true);

            _bodegaRepositorioMock
                .Setup(r => r.CrearAsync(bodega, restauranteId))
                .ReturnsAsync(bodegaCreada);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(bodega, restauranteId);

            // Validar
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            _bodegaRepositorioMock.Verify(r => r.CrearAsync(bodega, restauranteId), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task EjecutarAsync_NombreInvalido_LanzaArgumentException(string nombreInvalido)
        {
            // Preparar
            var restauranteId = 1;
            var bodega = new Bodega { Nombre = nombreInvalido, TipoBodegaId = 2 };

            // Ejecutar & Validar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => _casoDeUso.EjecutarAsync(bodega, restauranteId));
            Assert.Equal("El nombre de la bodega no puede estar vacío.", excepcion.Message);
            _bodegaRepositorioMock.Verify(r => r.CrearAsync(It.IsAny<Bodega>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_TipoBodegaInvalido_LanzaArgumentException()
        {
            // Preparar
            var restauranteId = 1;
            var bodega = new Bodega { Nombre = "Bodega Central", TipoBodegaId = 99 };

            _tipoBodegaRepositorioMock
                .Setup(r => r.ExisteAsync(bodega.TipoBodegaId))
                .ReturnsAsync(false);

            // Ejecutar & Validar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => _casoDeUso.EjecutarAsync(bodega, restauranteId));
            Assert.Equal("El tipo de bodega proporcionado no es válido.", excepcion.Message);
            _bodegaRepositorioMock.Verify(r => r.CrearAsync(It.IsAny<Bodega>(), It.IsAny<int>()), Times.Never);
        }
    }
}
