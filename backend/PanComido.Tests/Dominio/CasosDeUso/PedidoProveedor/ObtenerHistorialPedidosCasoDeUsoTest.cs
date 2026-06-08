using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.PedidoProveedor
{
    public class ObtenerHistorialPedidosCasoDeUsoTest
    {
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;

        public ObtenerHistorialPedidosCasoDeUsoTest() {
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorExiste_DevuelveListaDePedidos()
        {
            int proveedorId = 1;

            var pedidos = new List<DOM.Pedido>
            {
                new DOM.Pedido { Id = 1, ProveedorId = proveedorId, Estado = "Recibido" },
                new DOM.Pedido { Id = 2, ProveedorId = proveedorId, Estado = "Enviado" }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorId))
                .ReturnsAsync(new DOM.Proveedor { Id = proveedorId });

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidosPorProveedorAsync(proveedorId))
                .ReturnsAsync(pedidos);

            var casoDeUso = new ObtenerHistorialPedidosCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(proveedorId);
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_LanzaKeyNotFoundException()
        {
            int proveedorId = 1;

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorId))
                .ReturnsAsync((DOM.Proveedor?)null);


            var casoDeUso = new ObtenerHistorialPedidosCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => casoDeUso.EjecutarAsync(proveedorId));
        }
    }
}
