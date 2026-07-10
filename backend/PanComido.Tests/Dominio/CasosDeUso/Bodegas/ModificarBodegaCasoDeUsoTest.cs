using Moq;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Bodegas
{
    public class ModificarBodegaCasoDeUsoTest
    {
        private readonly Mock<IBodegaRepositorio> _bodegaRepositorioMock;
        private readonly Mock<ITipoBodegaRepositorio> _tipoBodegaRepositorioMock;
        private readonly ModificarBodegaCasoDeUso _casoDeUso;

        public ModificarBodegaCasoDeUsoTest()
        {
            _bodegaRepositorioMock = new Mock<IBodegaRepositorio>();
            _tipoBodegaRepositorioMock = new Mock<ITipoBodegaRepositorio>();
            _casoDeUso = new ModificarBodegaCasoDeUso(_bodegaRepositorioMock.Object, _tipoBodegaRepositorioMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_BodegaValidaYExistente_LlamaRepositorioYRetornaBodega()
        {
            // Preparar
            var restauranteId = 1;
            var bodegaModificada = new Bodega { Id = 10, Nombre = "Bodega Modificada", TipoBodegaId = 3 };
            var bodegaExistente = new Bodega { Id = 10, Nombre = "Bodega Vieja", TipoBodegaId = 2 };

            _tipoBodegaRepositorioMock
                .Setup(r => r.ExisteAsync(bodegaModificada.TipoBodegaId))
                .ReturnsAsync(true);

            _bodegaRepositorioMock
                .Setup(r => r.ObtenerBodegaPorIdAsync(10, restauranteId))
                .ReturnsAsync(bodegaExistente);

            _bodegaRepositorioMock
                .Setup(r => r.ModificarAsync(bodegaModificada, restauranteId))
                .ReturnsAsync(bodegaModificada);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(bodegaModificada, restauranteId);

            // Validar
            Assert.NotNull(resultado);
            Assert.Equal("Bodega Modificada", resultado.Nombre);
            _bodegaRepositorioMock.Verify(r => r.ObtenerBodegaPorIdAsync(10, restauranteId), Times.Once);
            _bodegaRepositorioMock.Verify(r => r.ModificarAsync(bodegaModificada, restauranteId), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task EjecutarAsync_NombreInvalido_LanzaArgumentException(string nombreInvalido)
        {
            // Preparar
            var restauranteId = 1;
            var bodegaModificada = new Bodega { Id = 10, Nombre = nombreInvalido, TipoBodegaId = 3 };

            // Ejecutar & Validar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => _casoDeUso.EjecutarAsync(bodegaModificada, restauranteId));
            Assert.Equal("El nombre de la bodega no puede estar vacío.", excepcion.Message);
            _bodegaRepositorioMock.Verify(r => r.ObtenerBodegaPorIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _bodegaRepositorioMock.Verify(r => r.ModificarAsync(It.IsAny<Bodega>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_TipoBodegaInvalido_LanzaArgumentException()
        {
            // Preparar
            var restauranteId = 1;
            var bodegaModificada = new Bodega { Id = 10, Nombre = "Bodega Modificada", TipoBodegaId = 99 };

            _tipoBodegaRepositorioMock
                .Setup(r => r.ExisteAsync(bodegaModificada.TipoBodegaId))
                .ReturnsAsync(false);

            // Ejecutar & Validar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => _casoDeUso.EjecutarAsync(bodegaModificada, restauranteId));
            Assert.Equal("El tipo de bodega proporcionado no es válido.", excepcion.Message);
            _bodegaRepositorioMock.Verify(r => r.ObtenerBodegaPorIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _bodegaRepositorioMock.Verify(r => r.ModificarAsync(It.IsAny<Bodega>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_BodegaNoExiste_LanzaKeyNotFoundException()
        {
            // Preparar
            var restauranteId = 1;
            var bodegaModificada = new Bodega { Id = 99, Nombre = "Bodega Modificada", TipoBodegaId = 3 };

            _tipoBodegaRepositorioMock
                .Setup(r => r.ExisteAsync(bodegaModificada.TipoBodegaId))
                .ReturnsAsync(true);

            _bodegaRepositorioMock
                .Setup(r => r.ObtenerBodegaPorIdAsync(99, restauranteId))
                .ReturnsAsync((Bodega)null);

            // Ejecutar & Validar
            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() => _casoDeUso.EjecutarAsync(bodegaModificada, restauranteId));
            Assert.Equal("La bodega que intenta modificar no existe.", excepcion.Message);
            _bodegaRepositorioMock.Verify(r => r.ModificarAsync(It.IsAny<Bodega>(), It.IsAny<int>()), Times.Never);
        }
    }
}
