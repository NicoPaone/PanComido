using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.PedidoProveedor
{
    public class EnviarPedidoProveedorCasoDeUsoTest
    {
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;

        public EnviarPedidoProveedorCasoDeUsoTest()
        {
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_EnviaElPedidoYDevuelveLinkWpp()
        {
            int pedidoId = 1;

            var itemsNuevos = new List<DOM.PedidoInsumo>
            {
              new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 5, PrecioCompra = 100 }
            };

            var pedidoExistente = new DOM.Pedido
            {
                Id = pedidoId,
                Estado = "Pendiente",
                ProveedorNombre = "Proveedor Test",
                ProveedorTelefono = "1150259552"
            };

            var pedidoConfirmado = new DOM.Pedido
            {
                Id = pedidoId,
                Estado = "Enviado",
                ProveedorNombre = "Proveedor Test",
                ProveedorTelefono = "1150259552",
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo { 
                        InsumoId = 10,
                        Cantidad = 5,
                        NombreInsumo = "Tomate",
                        UnidadMedida = "Kg" }
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedidoExistente);

            _pedidoRepoMock
                .Setup(r => r.EnviarPedidoAsync(pedidoId, It.IsAny<List<DOM.PedidoInsumo>>()))
                .ReturnsAsync(pedidoConfirmado);

            var casoDeUso = new EnviarPedidoProveedorCasoDeUso(
                _pedidoRepoMock.Object);

            var (pedido, linkWpp) = await casoDeUso.EjecutarAsync(pedidoId, pedidoConfirmado.ItemsInsumo);

            Assert.Equal("Enviado", pedido.Estado);
            Assert.StartsWith("https://wa.me/", linkWpp);

            _pedidoRepoMock.Verify(
                r => r.EnviarPedidoAsync(It.IsAny<int>(), It.IsAny<List<DOM.PedidoInsumo>>()));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPedidoNoExiste_LanzaKeyNotFoundException()
        {
            int pedidoId = 1;

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync((DOM.Pedido?)null);

            var casoDeUso = new EnviarPedidoProveedorCasoDeUso(
                _pedidoRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(pedidoId, new List<DOM.PedidoInsumo>()));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPedidoNoEstaEnEstadoPendiente_LanzaInvalidOperationException()
        {
            int pedidoId = 1;

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido
                {
                    Id = pedidoId,
                    Estado = "Enviado"
                });

            var casoDeUso = new EnviarPedidoProveedorCasoDeUso(
                _pedidoRepoMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(pedidoId, new List<DOM.PedidoInsumo>()));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayItems_LanzaArgumentException()
        {
            int pedidoId = 1;

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(new DOM.Pedido
                {
                    Id = pedidoId,
                    Estado = "Pendiente"
                });

            var casoDeUso = new EnviarPedidoProveedorCasoDeUso(
                _pedidoRepoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(pedidoId, new List<DOM.PedidoInsumo>()));
        }
    }
}
