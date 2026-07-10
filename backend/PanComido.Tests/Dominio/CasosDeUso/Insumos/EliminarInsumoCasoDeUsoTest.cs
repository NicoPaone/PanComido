using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class EliminarInsumoCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<IPlatoRepositorio> _platoRepoMock;
        private readonly Mock<IMiseAndPlaceRepositorio> _miseRepoMock;
        private readonly Mock<IBebidaPreparadaRepositorio> _bebidaRepoMock;
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly EliminarInsumoCasoDeUso _casoDeUso;

        public EliminarInsumoCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _platoRepoMock = new Mock<IPlatoRepositorio>();
            _miseRepoMock = new Mock<IMiseAndPlaceRepositorio>();
            _bebidaRepoMock = new Mock<IBebidaPreparadaRepositorio>();
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();

            _casoDeUso = new EliminarInsumoCasoDeUso(
                _insumoRepoMock.Object,
                _platoRepoMock.Object,
                _miseRepoMock.Object,
                _bebidaRepoMock.Object,
                _pedidoRepoMock.Object,
                _loteRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoPasaTodasLasValidaciones_LlamaAEliminar()
        {
            int insumoId = 10;
            int restauranteId = 1;

            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumo(insumoId)).ReturnsAsync(0);
            _platoRepoMock.Setup(r => r.ExisteInsumoEnPlatosActivosAsync(insumoId)).ReturnsAsync(false);
            _bebidaRepoMock.Setup(r => r.ExisteInsumoEnBebidasActivasAsync(insumoId)).ReturnsAsync(false);
            _miseRepoMock.Setup(r => r.ExisteInsumoEnMiseAndPlaceActivosAsync(insumoId)).ReturnsAsync(false);
            _pedidoRepoMock.Setup(r => r.ExisteInsumoEnPedidosPendientesAsync(insumoId)).ReturnsAsync(false);

            _insumoRepoMock.Setup(r => r.EliminarAsync(insumoId, restauranteId))
                .ReturnsAsync(new Insumo { Id = insumoId, Nombre = "Tomate" });

            await _casoDeUso.EjecutarAsync(insumoId, restauranteId);

            _insumoRepoMock.Verify(r => r.EliminarAsync(insumoId, restauranteId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayStock_LanzaInvalidOperationException()
        {
            int insumoId = 10;
            int restauranteId = 1;

            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumo(insumoId)).ReturnsAsync(5);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _casoDeUso.EjecutarAsync(insumoId, restauranteId));
            
            Assert.Contains("stock disponible en bodega", ex.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEstaEnPlatoActivo_LanzaInvalidOperationException()
        {
            int insumoId = 10;
            int restauranteId = 1;

            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumo(insumoId)).ReturnsAsync(0);
            _platoRepoMock.Setup(r => r.ExisteInsumoEnPlatosActivosAsync(insumoId)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _casoDeUso.EjecutarAsync(insumoId, restauranteId));
            
            Assert.Contains("receta de un Plato que está activo", ex.Message);
        }
        
        [Fact]
        public async Task EjecutarAsync_CuandoEstaEnPedidoPendiente_LanzaInvalidOperationException()
        {
            int insumoId = 10;
            int restauranteId = 1;

            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumo(insumoId)).ReturnsAsync(0);
            _platoRepoMock.Setup(r => r.ExisteInsumoEnPlatosActivosAsync(insumoId)).ReturnsAsync(false);
            _bebidaRepoMock.Setup(r => r.ExisteInsumoEnBebidasActivasAsync(insumoId)).ReturnsAsync(false);
            _miseRepoMock.Setup(r => r.ExisteInsumoEnMiseAndPlaceActivosAsync(insumoId)).ReturnsAsync(false);
            _pedidoRepoMock.Setup(r => r.ExisteInsumoEnPedidosPendientesAsync(insumoId)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _casoDeUso.EjecutarAsync(insumoId, restauranteId));
            
            Assert.Contains("Pedidos a Proveedores que lo incluyen", ex.Message);
        }
    }
}
