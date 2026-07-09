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
    public class EliminarBodegaCasoDeUsoTest
    {
        private readonly Mock<IBodegaRepositorio> _bodegaRepositorioMock;
        private readonly EliminarBodegaCasoDeUso _casoDeUso;

        public EliminarBodegaCasoDeUsoTest()
        {
            _bodegaRepositorioMock = new Mock<IBodegaRepositorio>();
            _casoDeUso = new EliminarBodegaCasoDeUso(_bodegaRepositorioMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_BodegaExisteYSinLotes_LlamaEliminarAsync()
        {
            // Preparar
            var bodegaId = 5;
            var restauranteId = 1;
            var bodegaExistente = new Bodega { Id = bodegaId, Nombre = "Bodega a Eliminar" };

            _bodegaRepositorioMock
                .Setup(r => r.ObtenerBodegaPorIdAsync(bodegaId, restauranteId))
                .ReturnsAsync(bodegaExistente);

            _bodegaRepositorioMock
                .Setup(r => r.TieneLotesAsociadosAsync(bodegaId))
                .ReturnsAsync(false);

            // Ejecutar
            await _casoDeUso.EjecutarAsync(bodegaId, restauranteId);

            // Validar
            _bodegaRepositorioMock.Verify(r => r.ObtenerBodegaPorIdAsync(bodegaId, restauranteId), Times.Once);
            _bodegaRepositorioMock.Verify(r => r.TieneLotesAsociadosAsync(bodegaId), Times.Once);
            _bodegaRepositorioMock.Verify(r => r.EliminarAsync(bodegaId, restauranteId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_BodegaNoExiste_LanzaKeyNotFoundException()
        {
            // Preparar
            var bodegaId = 99;
            var restauranteId = 1;

            _bodegaRepositorioMock
                .Setup(r => r.ObtenerBodegaPorIdAsync(bodegaId, restauranteId))
                .ReturnsAsync((Bodega)null);

            // Ejecutar & Validar
            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() => _casoDeUso.EjecutarAsync(bodegaId, restauranteId));
            Assert.Equal("La bodega que intenta eliminar no existe.", excepcion.Message);
            
            _bodegaRepositorioMock.Verify(r => r.TieneLotesAsociadosAsync(It.IsAny<int>()), Times.Never);
            _bodegaRepositorioMock.Verify(r => r.EliminarAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_BodegaTieneLotes_LanzaInvalidOperationException()
        {
            // Preparar
            var bodegaId = 5;
            var restauranteId = 1;
            var bodegaExistente = new Bodega { Id = bodegaId, Nombre = "Bodega Llena" };

            _bodegaRepositorioMock
                .Setup(r => r.ObtenerBodegaPorIdAsync(bodegaId, restauranteId))
                .ReturnsAsync(bodegaExistente);

            _bodegaRepositorioMock
                .Setup(r => r.TieneLotesAsociadosAsync(bodegaId))
                .ReturnsAsync(true);

            // Ejecutar & Validar
            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(() => _casoDeUso.EjecutarAsync(bodegaId, restauranteId));
            Assert.Equal("No se puede eliminar la bodega porque contiene lotes físicos (mercadería) asociados.", excepcion.Message);
            
            _bodegaRepositorioMock.Verify(r => r.EliminarAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }
}
