using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class ObtenerPedidosPorProveedorCasoDeUsoTest
    {
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;

        public ObtenerPedidosPorProveedorCasoDeUsoTest()
        {
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_LanzaKeyNotFoundException()
        {
            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(999))
                .ReturnsAsync((DOM.Proveedor?)null);

            var casoDeUso = new ObtenerHistorialPedidosCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(999));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoProveedorExistePeroNoTienePedidos_DevuelveListaVacia()
        {
            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(4))
                .ReturnsAsync(new DOM.Proveedor { Id = 4, Nombre = "Lácteos del Campo" });

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidosPorProveedorAsync(4))
                .ReturnsAsync(new List<DOM.Pedido>());

            var casoDeUso = new ObtenerHistorialPedidosCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(4);
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoProveedorExisteYTienePedidos_DevuelveListaDePedidos()
        {
            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(2))
                .ReturnsAsync(new DOM.Proveedor { Id = 2, Nombre = "Carnicería El Gaucho" });

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidosPorProveedorAsync(2))
                .ReturnsAsync(new List<DOM.Pedido>
                {
                    new DOM.Pedido { Id = 10, Fecha = new DateOnly(2026, 5, 27), Estado = "Recibido" },
                    new DOM.Pedido { Id = 11, Fecha = new DateOnly(2026, 5, 10), Estado = "Pendiente" }
                });

            var casoDeUso = new ObtenerHistorialPedidosCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(2);
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal(10, resultado[0].Id);
            Assert.Equal("Recibido", resultado[0].Estado);
            Assert.Equal(11, resultado[1].Id);
            Assert.Equal("Pendiente", resultado[1].Estado);
        }
    }
}