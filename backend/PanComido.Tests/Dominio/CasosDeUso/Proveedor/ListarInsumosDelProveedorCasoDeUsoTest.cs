using Moq;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class ListarInsumosDelProveedorCasoDeUsoTests
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;

        public ListarInsumosDelProveedorCasoDeUsoTests()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayInsumos_DevuelveLaLista()
        {
            int proveedorId = 1;
            int restauranteId = 1;
            var insumos = new List<Insumo>
            {
                new Insumo { Id = 1, Nombre = "Tomate"},
                new Insumo { Id = 2, Nombre = "Lechuga"}
            };

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId))
                .ReturnsAsync(insumos);

            var casoDeUso = new ListarInsumosDelProveedorCasoDeUso(_insumoRepoMock.Object);
            var resultado = await casoDeUso.EjecutarAsync(proveedorId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Tomate", resultado[0].Nombre);
            Assert.Equal("Lechuga", resultado[1].Nombre);

            _insumoRepoMock.Verify(
                r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId),
                Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayInsumos_DevuelveListaVacia()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId))
                .ReturnsAsync(new List<Insumo>());

            var casoDeUso = new ListarInsumosDelProveedorCasoDeUso(_insumoRepoMock.Object);
            var resultado = await casoDeUso.EjecutarAsync(proveedorId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            _insumoRepoMock.Verify(
                r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId),
                Times.Once);
        }
    }
}