using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.PedidoProveedor
{
    public class CrearPedidoCasoDeUsoTests
    {
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<ILogger<CrearPedidoCasoDeUso>> _loggerMock;

        public CrearPedidoCasoDeUsoTests()
        {
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _loggerMock = new Mock<ILogger<CrearPedidoCasoDeUso>>();
        }

        private CrearPedidoCasoDeUso CrearCasoDeUso() =>
            new CrearPedidoCasoDeUso(
                _pedidoRepoMock.Object,
                _proveedorRepoMock.Object,
                _insumoRepoMock.Object,
                _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_CreaElPedido()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var insumos = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate" },
                new DOM.Insumo { Id = 11, Nombre = "Lechuga" }
            };

            var proveedor = new DOM.Proveedor { Id = proveedorId, RestauranteId = restauranteId };

            var pedido = new DOM.Pedido
            {
                ProveedorId = proveedorId,
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 5, PrecioCompra = 100 },
                    new DOM.PedidoInsumo { InsumoId = 11, Cantidad = 3, PrecioCompra = 50 }
                }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorId))
                .ReturnsAsync(proveedor);

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId))
                .ReturnsAsync(insumos);

            _pedidoRepoMock
                .Setup(r => r.CrearPedidoAsync(It.IsAny<DOM.Pedido>()))
                .ReturnsAsync((DOM.Pedido p) => { p.Id = 100; return p; });

            var resultado = await CrearCasoDeUso().EjecutarAsync(pedido, restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(100, resultado.Id);
            Assert.Equal(EstadoPedido.Pendiente, resultado.Estado);
            _pedidoRepoMock.Verify(r => r.CrearPedidoAsync(It.IsAny<DOM.Pedido>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_LanzaKeyNotFoundException()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var pedido = new DOM.Pedido
            {
                ProveedorId = proveedorId,
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 5, PrecioCompra = 100 }
                }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorId))
                .ReturnsAsync((DOM.Proveedor?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(pedido, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorEsDeOtroRestaurante_LanzaKeyNotFoundException()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var pedido = new DOM.Pedido
            {
                ProveedorId = proveedorId,
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 5, PrecioCompra = 100 }
                }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorId))
                .ReturnsAsync(new DOM.Proveedor { Id = proveedorId, RestauranteId = 99 });

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(pedido, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayItemsDuplicados_LanzaInvalidOperationException()
        {
            var pedido = new DOM.Pedido
            {
                ProveedorId = 1,
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 5, PrecioCompra = 100 },
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 3, PrecioCompra = 50 }
                }
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => CrearCasoDeUso().EjecutarAsync(pedido, 1));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoUnInsumoNoPerteneceAlProveedor_LanzaArgumentException()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var proveedor = new DOM.Proveedor { Id = proveedorId, RestauranteId = restauranteId };
            var insumos = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate" },
                new DOM.Insumo { Id = 11, Nombre = "Lechuga" }
            };

            var pedido = new DOM.Pedido
            {
                ProveedorId = proveedorId,
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo { InsumoId = 99, Cantidad = 5, PrecioCompra = 100 },
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 3, PrecioCompra = 50 }
                }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorId))
                .ReturnsAsync(proveedor);

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId))
                .ReturnsAsync(insumos);

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(pedido, restauranteId));
        }
    }
}