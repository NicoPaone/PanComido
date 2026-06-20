using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.PedidoProveedor
{
    public class EnviarPedidoProveedorCasoDeUsoTest
    {
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        private readonly Mock<ILogger<EnviarPedidoProveedorCasoDeUso>> _loggerMock;

        public EnviarPedidoProveedorCasoDeUsoTest()
        {
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _loggerMock = new Mock<ILogger<EnviarPedidoProveedorCasoDeUso>>();
        }

        private EnviarPedidoProveedorCasoDeUso CrearCasoDeUso() =>
            new EnviarPedidoProveedorCasoDeUso(_pedidoRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_EnviaElPedidoYDevuelveLinkWpp()
        {
            int pedidoId = 1;

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
                    new DOM.PedidoInsumo { InsumoId = 10, Cantidad = 5, NombreInsumo = "Tomate", UnidadMedida = "Kg" }
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedidoExistente);

            _pedidoRepoMock
                .Setup(r => r.EnviarPedidoAsync(pedidoId, It.IsAny<List<DOM.PedidoInsumo>>()))
                .ReturnsAsync(pedidoConfirmado);

            var (pedido, linkWpp) = await CrearCasoDeUso().EjecutarAsync(pedidoId, pedidoConfirmado.ItemsInsumo);

            Assert.Equal("Enviado", pedido.Estado);
            Assert.StartsWith("https://wa.me/", linkWpp);
            _pedidoRepoMock.Verify(r => r.EnviarPedidoAsync(It.IsAny<int>(), It.IsAny<List<DOM.PedidoInsumo>>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPedidoNoExiste_LanzaKeyNotFoundException()
        {
            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(1))
                .ReturnsAsync((DOM.Pedido?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(1, new List<DOM.PedidoInsumo>()));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPedidoNoEstaEnEstadoPendiente_LanzaInvalidOperationException()
        {
            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(1))
                .ReturnsAsync(new DOM.Pedido { Id = 1, Estado = "Enviado" });

            await Assert.ThrowsAsync<InvalidOperationException>(() => CrearCasoDeUso().EjecutarAsync(1, new List<DOM.PedidoInsumo>()));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayItems_LanzaArgumentException()
        {
            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(1))
                .ReturnsAsync(new DOM.Pedido { Id = 1, Estado = "Pendiente" });

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(1, new List<DOM.PedidoInsumo>()));
        }
    }
}