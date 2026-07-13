using Microsoft.Extensions.Logging;
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
        private readonly Mock<ILogger<RecibirPedidoProveedorCasoDeUso>> _loggerMock;

        public RecibirPedidoProveedorCasoDeUsoTest()
        {
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _bodegaRepoMock = new Mock<IBodegaRepositorio>();
            _loggerMock = new Mock<ILogger<RecibirPedidoProveedorCasoDeUso>>();
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

            var itemsConPrecioConfirmado = new List<DOM.PedidoInsumo>
            {
                new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 150 }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido
                {
                    Id = pedidoId,
                    Estado = EstadoPedidoProveedor.Enviado,
                    ItemsInsumo = new List<DOM.PedidoInsumo> { new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 140 } }
                });

            _bodegaRepoMock
                .Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, 1))
                .ReturnsAsync(true);

            _loteRepoMock
                .Setup(r => r.CrearLotesAsync(It.IsAny<List<DOM.Lote>>()))
                .ReturnsAsync(lotes);

            _pedidoRepoMock
                .Setup(r => r.MarcarComoRecibidoAsync(pedidoId, itemsConPrecioConfirmado))
                .Returns(Task.CompletedTask);

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object,
                 _loggerMock.Object);

            await casoDeUso.EjecutarAsync(pedidoId, lotes, itemsConPrecioConfirmado, restauranteId);

            _pedidoRepoMock.Verify(r => r.MarcarComoRecibidoAsync(pedidoId, itemsConPrecioConfirmado), Times.Once);
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

            var itemsConPrecioConfirmado = new List<DOM.PedidoInsumo>
            {
                new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 150 }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync((DOM.Pedido?)null);

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object,
                 _loggerMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, itemsConPrecioConfirmado, restauranteId));
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

            var itemsConPrecioConfirmado = new List<DOM.PedidoInsumo>
            {
                new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 150 }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido { Id = pedidoId, Estado = EstadoPedidoProveedor.Pendiente });

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object,
                 _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, itemsConPrecioConfirmado, restauranteId));

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

            var itemsConPrecioConfirmado = new List<DOM.PedidoInsumo>
            {
                new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 150 }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido { Id = pedidoId, Estado = EstadoPedidoProveedor.Enviado });

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object,
                 _loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, itemsConPrecioConfirmado, restauranteId));

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

            var itemsConPrecioConfirmado = new List<DOM.PedidoInsumo>
            {
                new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 150 }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido { Id = pedidoId, Estado = EstadoPedidoProveedor.Enviado });

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object,
                 _loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, itemsConPrecioConfirmado, restauranteId));
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

            var itemsConPrecioConfirmado = new List<DOM.PedidoInsumo>
            {
                new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 150 }
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
                 _bodegaRepoMock.Object,
                 _loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, itemsConPrecioConfirmado, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPrecioDeCompraEsCero_LanzaArgumentException()
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

            var itemsConPrecioConfirmado = new List<DOM.PedidoInsumo>
            {
                new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 0 }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido
                {
                    Id = pedidoId,
                    Estado = EstadoPedidoProveedor.Enviado,
                    ItemsInsumo = new List<DOM.PedidoInsumo> { new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 140 } }
                });

            _bodegaRepoMock
                .Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, 1))
                .ReturnsAsync(true);

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object,
                 _loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, itemsConPrecioConfirmado, restauranteId));

            _pedidoRepoMock.Verify(r => r.MarcarComoRecibidoAsync(It.IsAny<int>(), It.IsAny<List<DOM.PedidoInsumo>>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaCantidadDePreciosConfirmadosNoCoincideConLosItemsDelPedido_LanzaInvalidOperationException()
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

            var itemsConPrecioConfirmado = new List<DOM.PedidoInsumo>
            {
                new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 150 }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido
                {
                    Id = pedidoId,
                    Estado = EstadoPedidoProveedor.Enviado,
                    ItemsInsumo = new List<DOM.PedidoInsumo>
                    {
                        new DOM.PedidoInsumo { InsumoId = 10, PrecioCompra = 140 },
                        new DOM.PedidoInsumo { InsumoId = 11, PrecioCompra = 90 }
                    }
                });

            _bodegaRepoMock
                .Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, 1))
                .ReturnsAsync(true);

            var casoDeUso = new RecibirPedidoProveedorCasoDeUso(
                 _pedidoRepoMock.Object,
                 _loteRepoMock.Object,
                 _bodegaRepoMock.Object,
                 _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(pedidoId, lotes, itemsConPrecioConfirmado, restauranteId));

            _pedidoRepoMock.Verify(r => r.MarcarComoRecibidoAsync(It.IsAny<int>(), It.IsAny<List<DOM.PedidoInsumo>>()), Times.Never);
        }
    }
}
