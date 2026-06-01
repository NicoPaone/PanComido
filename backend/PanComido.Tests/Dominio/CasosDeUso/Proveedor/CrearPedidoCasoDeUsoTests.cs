using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class CrearPedidoCasoDeUsoTests
    {
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;

        public CrearPedidoCasoDeUsoTests()
        {
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
        }

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

            var proveedor = new DOM.Proveedor
            {
                Id = proveedorId,
                RestauranteId = restauranteId
            };

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

            var casoDeUso = new CrearPedidoCasoDeUso(
                _pedidoRepoMock.Object,
                _proveedorRepoMock.Object,
                _insumoRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(pedido, restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(100, resultado.Id);
            Assert.Equal("Pendiente", resultado.Estado);
            Assert.Equal(2, resultado.ItemsInsumo.Count);

            _pedidoRepoMock.Verify(
                r => r.CrearPedidoAsync(It.IsAny<DOM.Pedido>()),
                Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_LanzaExcepcion()
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

            var casoDeUso = new CrearPedidoCasoDeUso(
                _pedidoRepoMock.Object,
                _proveedorRepoMock.Object,
                _insumoRepoMock.Object);

            var excepcion = await Assert.ThrowsAsync<Exception>(
                () => casoDeUso.EjecutarAsync(pedido, restauranteId));

            Assert.Equal("Proveedor no encontrado", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorEsDeOtroRestaurante_LanzaExcepcion()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var proveedorDeOtroRestaurante = new DOM.Proveedor
            {
                Id = proveedorId,
                RestauranteId = 99
            };

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
                .ReturnsAsync(proveedorDeOtroRestaurante);

            var casoDeUso = new CrearPedidoCasoDeUso(
                _pedidoRepoMock.Object,
                _proveedorRepoMock.Object,
                _insumoRepoMock.Object);

            var excepcion = await Assert.ThrowsAsync<Exception>(
                () => casoDeUso.EjecutarAsync(pedido, restauranteId));

            Assert.Equal("Proveedor no encontrado", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayItemsDuplicados_LanzaExcepcion()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var pedido = new DOM.Pedido
            {
                ProveedorId = proveedorId,
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 5, PrecioCompra = 100 },
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 3, PrecioCompra = 50 }
                }
            };

            var casoDeUso = new CrearPedidoCasoDeUso(
                _pedidoRepoMock.Object,
                _proveedorRepoMock.Object,
                _insumoRepoMock.Object);

            var excepcion = await Assert.ThrowsAsync<Exception>(
                () => casoDeUso.EjecutarAsync(pedido, restauranteId));

            Assert.Equal("Hay insumos duplicados", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoUnInsumoNoPerteneceAlProveedor_LanzaExcepcion()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var proveedor = new DOM.Proveedor
            {
                Id = proveedorId,
                RestauranteId = restauranteId
            };

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

            var casoDeUso = new CrearPedidoCasoDeUso(
                _pedidoRepoMock.Object,
                _proveedorRepoMock.Object,
                _insumoRepoMock.Object);

            var excepcion = await Assert.ThrowsAsync<Exception>(
                () => casoDeUso.EjecutarAsync(pedido, restauranteId));

            Assert.Equal("Hay insumos que no pertenecen al proveedor", excepcion.Message);
        }
    }
}