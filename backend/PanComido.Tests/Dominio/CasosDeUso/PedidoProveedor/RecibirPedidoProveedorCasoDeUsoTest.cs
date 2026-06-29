using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;


namespace PanComido.Tests.Dominio.CasosDeUso.PedidoProveedor
{
    public class RecibirPedidoProveedorCasoDeUsoTest
    {
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IBodegaRepositorio> _bodegaRepoMock;

        public RecibirPedidoProveedorCasoDeUsoTest()
        {
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _bodegaRepoMock = new Mock<IBodegaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_RecibeElPedido()
        {
            int pedidoId = 1;
            int restauranteId = 1;

            var lotes = new List<DOM.Lote>
            {
                new DOM.Lote
                {
                    Nombre = "Lote Tomate",
                    Cantidad = 5,
                    BodegaId = 1,
                    FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido { Id = pedidoId, Estado = EstadoPedidoProveedor.Enviado });

            _bodegaRepoMock
                .Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, 1))
                .ReturnsAsync(true);

            _loteRepoMock
                .Setup(r => r.CrearLotesAsync(It.IsAny<List<DOM.Lote>>()))
                .Returns(Task.CompletedTask);

            _pedidoRepoMock
                .Setup(r => r.MarcarComoRecibidoAsync(pedidoId))
                .Returns(Task.CompletedTask);

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object);

            await casoDeUso.EjecutarAsync(pedidoId, lotes, restauranteId);

            _pedidoRepoMock.Verify(r => r.MarcarComoRecibidoAsync(pedidoId), Times.Once);
            _loteRepoMock.Verify(r => r.CrearLotesAsync(It.IsAny<List<DOM.Lote>>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPedidoNoExiste_LanzaKeyNotFoundException()
        {
            int pedidoId = 1;
            int restauranteId = 1;

            var lotes = new List<DOM.Lote>
            {
                new DOM.Lote
                {
                    Nombre = "Lote Tomate",
                    Cantidad = 5,
                    BodegaId = 1,
                    FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync((DOM.Pedido?)null);

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPedidoNoEstaEnEstadoEnviado_LanzaInvalidOperationException()
        {
            int pedidoId = 1;
            int restauranteId = 1;

            var lotes = new List<DOM.Lote>
            {
                new DOM.Lote
                {
                    Nombre = "Lote Tomate",
                    Cantidad = 2,
                    BodegaId = 1,
                    FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido { Id = pedidoId, Estado = EstadoPedidoProveedor.Pendiente });

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, restauranteId));

        }

        [Fact]
        public async Task EjecutarAsync_CuandoFechaVencimientoEsPasada_LanzaArgumentException()
        {
            int pedidoId = 1;
            int restauranteId = 1;

            var lotes = new List<DOM.Lote>
            {
                new DOM.Lote
                {
                    Nombre = "Lote Tomate",
                    Cantidad = 5,
                    BodegaId = 1,
                    FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido { Id = pedidoId, Estado = EstadoPedidoProveedor.Enviado });

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, restauranteId));

        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaCantidadEsCero_LanzaArgumentException()
        {
            int pedidoId = 1;
            int restauranteId = 1;

            var lotes = new List<DOM.Lote>
            {
                new DOM.Lote
                {
                    Nombre = "Lote Tomate",
                    Cantidad = 0,
                    BodegaId = 1,
                    FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido { Id = pedidoId, Estado = EstadoPedidoProveedor.Enviado });

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaBodegaNoEsValida_LanzaArgumentException()
        {
            int pedidoId = 1;
            int restauranteId = 1;

            var lotes = new List<DOM.Lote>
            {
                new DOM.Lote
                {
                    Nombre = "Lote Tomate",
                    Cantidad = 5,
                    BodegaId = 1,
                    FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido { Id = pedidoId, Estado = EstadoPedidoProveedor.Enviado });

            _bodegaRepoMock
                .Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, 1))
                .ReturnsAsync(false);

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, restauranteId));
        }
    }
}
