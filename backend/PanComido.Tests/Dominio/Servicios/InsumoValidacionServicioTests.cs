using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Servicios;

namespace PanComido.Tests.Dominio.Servicios
{
    public class InsumoValidacionServicioTests
    {
        private readonly Mock<ICategoriaInsumoRepositorio> _categoriaRepoMock;
        private readonly Mock<IUnidadMedidaRepositorio> _unidadMedidaRepoMock;
        private readonly InsumoValidacionServicio _servicio;

        public InsumoValidacionServicioTests()
        {
            _categoriaRepoMock = new Mock<ICategoriaInsumoRepositorio>();
            _unidadMedidaRepoMock = new Mock<IUnidadMedidaRepositorio>();
            _servicio = new InsumoValidacionServicio(
                _categoriaRepoMock.Object,
                _unidadMedidaRepoMock.Object,
                Mock.Of<ILogger<InsumoValidacionServicio>>());
        }

        [Fact]
        public async Task ObtenerYValidarCategoriaAsync_CuandoExiste_DevuelveLaCategoria()
        {
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, Descripcion = "Secos" };
            _categoriaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(categoria);

            CategoriaInsumo resultado = await _servicio.ObtenerYValidarCategoriaAsync(1);

            Assert.Same(categoria, resultado);
        }

        [Fact]
        public async Task ObtenerYValidarCategoriaAsync_CuandoNoExiste_LanzaArgumentException()
        {
            _categoriaRepoMock.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((CategoriaInsumo)null);

            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicio.ObtenerYValidarCategoriaAsync(99));

            Assert.Equal("La categoría de insumo seleccionada no existe en el sistema.", excepcion.Message);
        }

        [Fact]
        public async Task ObtenerYValidarUnidadMedidaAsync_CuandoExiste_DevuelveLaUnidadMedida()
        {
            UnidadMedida unidadMedida = new UnidadMedida { Id = 1, Nombre = "Kilos" };
            _unidadMedidaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(unidadMedida);

            UnidadMedida resultado = await _servicio.ObtenerYValidarUnidadMedidaAsync(1);

            Assert.Same(unidadMedida, resultado);
        }

        [Fact]
        public async Task ObtenerYValidarUnidadMedidaAsync_CuandoNoExiste_LanzaArgumentException()
        {
            _unidadMedidaRepoMock.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((UnidadMedida)null);

            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicio.ObtenerYValidarUnidadMedidaAsync(99));

            Assert.Equal("La unidad de medida seleccionada no existe en el sistema.", excepcion.Message);
        }
    }
}
