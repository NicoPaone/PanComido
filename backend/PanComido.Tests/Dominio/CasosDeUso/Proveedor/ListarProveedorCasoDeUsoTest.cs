using Moq;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class ListarProveedorCasoDeUsoTests
    {
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        public ListarProveedorCasoDeUsoTests()
        {
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayProveedores_DevuelveListaVacia()
        {
            // Devolver lista vacia para restauranteId = 1
            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedoresAsync(1))
                .ReturnsAsync(new List<global::PanComido.Dominio.Entidades.Proveedor>());

            // Caso de uso con los mocks
            var casoDeUso = new ListarProveedorCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            // metodo a probar
            var resultado = await casoDeUso.EjecutarAsync(1);

            // Verificacion
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayProveedores_DevuelveListaOrdenadaPorFechaUltimoPedido()
        {
            //
            var proveedores = new List<global::PanComido.Dominio.Entidades.Proveedor>
            {
                new() { Id = 1, Nombre = "Verdulería Don José" },
                new() { Id = 2, Nombre = "Carnicería El Gaucho" },
                new() { Id = 3, Nombre = "Distribuidora Central" }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedoresAsync(1))
                .ReturnsAsync(proveedores);

            //
            _pedidoRepoMock
                .Setup(r => r.ObtenerFechaUltimoPedidoDeProveedorAsync(1))
                .ReturnsAsync(new DateOnly(2026, 5, 15));
            _pedidoRepoMock
                .Setup(r => r.ObtenerFechaUltimoPedidoDeProveedorAsync(2))
                .ReturnsAsync(new DateOnly(2026, 5, 26));
            _pedidoRepoMock
                .Setup(r => r.ObtenerFechaUltimoPedidoDeProveedorAsync(3))
                .ReturnsAsync(new DateOnly(2026, 5, 20));

            //
            var casoDeUso = new ListarProveedorCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            //
            var resultado = await casoDeUso.EjecutarAsync(1);

            // Verificacion
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count);
            Assert.Equal(2, resultado[0].Id);
            Assert.Equal(3, resultado[1].Id);
            Assert.Equal(1, resultado[2].Id);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoUnProveedorNoTienePedidos_LoColocaAlFinalConFechaNull()
        {
            // Proveedor sin pedidos
            var proveedores = new List<global::PanComido.Dominio.Entidades.Proveedor>
            {
                new() { Id = 1, Nombre = "Verdulería Don José" },
                new() { Id = 2, Nombre = "Lácteos del Campo" }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedoresAsync(1))
                .ReturnsAsync(proveedores);
            _pedidoRepoMock
                .Setup(r => r.ObtenerFechaUltimoPedidoDeProveedorAsync(1))
                .ReturnsAsync(new DateOnly(2026, 5, 15));
            _pedidoRepoMock
                .Setup(r => r.ObtenerFechaUltimoPedidoDeProveedorAsync(2))
                .ReturnsAsync((DateOnly?)null);

            var casoDeUso = new ListarProveedorCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(1);

            Assert.Equal(1, resultado[0].Id);
            Assert.NotNull(resultado[0].FechaUltimoPedido);


            // Verificacion
            Assert.Equal(2, resultado[1].Id);
            Assert.Null(resultado[1].FechaUltimoPedido);
        }

    }
}