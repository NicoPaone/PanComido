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
        private object proveedorRepoMock;

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
                new global::PanComido.Dominio.Entidades.Proveedor { Id = 1, Nombre = "Proveedor A" },
                new global::PanComido.Dominio.Entidades.Proveedor { Id = 2, Nombre = "Proveedor B" }
            };
            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedoresAsync(1))
                .ReturnsAsync(proveedores);

            //
            _pedidoRepoMock
                .Setup(r => r.ObtenerFechaUltimoPedidoDeProveedorAsync(1))
                .ReturnsAsync(new DateOnly(2024, 6, 1));
            _pedidoRepoMock
                .Setup(r => r.ObtenerFechaUltimoPedidoDeProveedorAsync(2))
                .ReturnsAsync(new DateOnly(2024, 5, 1));

            //
            var casoDeUso = new ListarProveedorCasoDeUso(
                _proveedorRepoMock.Object,
                _pedidoRepoMock.Object);

            //
            var resultado = await casoDeUso.EjecutarAsync(1);

            // Verificacion
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Proveedor A", resultado[0].Nombre);
            Assert.Equal("Proveedor B", resultado[1].Nombre);
        }
    }
}