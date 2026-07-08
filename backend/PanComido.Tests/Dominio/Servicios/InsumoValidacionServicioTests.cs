using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Servicios;

namespace PanComido.Tests.Dominio.Servicios
{
    public class InsumoValidacionServicioTests
    {
        private readonly Mock<ICategoriaInsumoRepositorio> _categoriaRepoMock;
        private readonly Mock<IUnidadMedidaRepositorio> _unidadMedidaRepoMock;
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly InsumoValidacionServicio _servicio;

        public InsumoValidacionServicioTests()
        {
            _categoriaRepoMock = new Mock<ICategoriaInsumoRepositorio>();
            _unidadMedidaRepoMock = new Mock<IUnidadMedidaRepositorio>();
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _servicio = new InsumoValidacionServicio(
                _categoriaRepoMock.Object,
                _unidadMedidaRepoMock.Object,
                _insumoRepoMock.Object,
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

        [Fact]
        public async Task ValidarInsumosDeRecetaBebidaAsync_CuandoTodosSonTipoBebida_NoLanzaExcepcion()
        {
            int restauranteId = 1;
            var insumos = new List<BebidaPreparadaInsumo>
            {
                new BebidaPreparadaInsumo { InsumoId = 10, Cantidad = 100 },
                new BebidaPreparadaInsumo { InsumoId = 11, Cantidad = 300 }
            };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(10, restauranteId))
                .ReturnsAsync(new Insumo { Id = 10, Nombre = "Fernet", Tipo = TipoInsumo.Bebida });
            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(11, restauranteId))
                .ReturnsAsync(new Insumo { Id = 11, Nombre = "Coca", Tipo = TipoInsumo.Bebida });

            await _servicio.ValidarInsumosDeRecetaBebidaAsync(restauranteId, insumos);
        }

        [Fact]
        public async Task ValidarInsumosDeRecetaBebidaAsync_CuandoUnInsumoNoExiste_LanzaArgumentException()
        {
            int restauranteId = 1;
            var insumos = new List<BebidaPreparadaInsumo>
            {
                new BebidaPreparadaInsumo { InsumoId = 99, Cantidad = 100 }
            };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(99, restauranteId)).ReturnsAsync((Insumo)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicio.ValidarInsumosDeRecetaBebidaAsync(restauranteId, insumos));
        }

        [Fact]
        public async Task ValidarInsumosDeRecetaBebidaAsync_CuandoUnInsumoEsTipoIngrediente_LanzaArgumentException()
        {
            int restauranteId = 1;
            var insumos = new List<BebidaPreparadaInsumo>
            {
                new BebidaPreparadaInsumo { InsumoId = 20, Cantidad = 100 }
            };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(20, restauranteId))
                .ReturnsAsync(new Insumo { Id = 20, Nombre = "Tomate", Tipo = TipoInsumo.Ingrediente });

            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicio.ValidarInsumosDeRecetaBebidaAsync(restauranteId, insumos));

            Assert.Equal("El insumo 'Tomate' no es de tipo Bebida y no puede usarse en la receta de una bebida preparada.", excepcion.Message);
        }
    }
}
