using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_DevuelveNull()
        {
            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(999))
                .ReturnsAsync((global::PanComido.Dominio.Entidades.Proveedor?)null);

            var pedidoRepoMock = new Mock<IPedidoRepositorio>();

            var casoDeUso = new ObtenerHistorialPedidosCasoDeUso(
                _proveedorRepoMock.Object,
                pedidoRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoProveedorExistePeroNoTienePedidos_DevuelveListaVacia()
        {
            var proveedor = new global::PanComido.Dominio.Entidades.Proveedor
            {
                Id = 4,
                Nombre = "Lácteos del Campo"
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(4))
                .ReturnsAsync(proveedor);

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidosPorProveedorAsync(4))
                .ReturnsAsync(new List<Pedido>());

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
            var proveedor = new global::PanComido.Dominio.Entidades.Proveedor
            {
                Id = 2,
                Nombre = "Carnicería El Gaucho"
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(2))
                .ReturnsAsync(proveedor);

            var pedidos = new List<Pedido>
            {
                new() { Id = 10, Fecha = new DateOnly(2026, 5, 27), Estado = "Recibido" },
                new() { Id = 11, Fecha = new DateOnly(2026, 5, 10), Estado = "Pendiente" }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidosPorProveedorAsync(2))
                .ReturnsAsync(pedidos);

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
