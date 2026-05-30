using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class CrearInsumoCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IBodegaRepositorio> _bodegaRepoMock;
        private readonly Mock<IUnidadMedidaRepositorio> _unidadMedidaRepoMock;
        private readonly Mock<ICategoriaInsumoRepositorio> _categoriaRepoMock;

        public CrearInsumoCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _bodegaRepoMock = new Mock<IBodegaRepositorio>();
            _unidadMedidaRepoMock = new Mock<IUnidadMedidaRepositorio>();
            _categoriaRepoMock = new Mock<ICategoriaInsumoRepositorio>();
        }

        private CrearInsumoCasoDeUso CrearCasoDeUsoConReposMock()
        {
            return new CrearInsumoCasoDeUso(
                _insumoRepoMock.Object,
                _loteRepoMock.Object,
                _bodegaRepoMock.Object,
                _unidadMedidaRepoMock.Object,
                _categoriaRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodosLosDatosSonValidos_CreaInsumoConSuLote()
        {
            // 1. Preparar
            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            DateOnly fechaVencimientoFutura = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10);

            Insumo insumo = new Insumo { Nombre = "Harina", CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5 };
            CategoriaInsumo categoriaValida = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente };

            _categoriaRepoMock.Setup(r => r.ObtenerPorIdAsync(insumo.CategoriaId)).ReturnsAsync(categoriaValida);
            _unidadMedidaRepoMock.Setup(r => r.ExisteAsync(insumo.UnidadDeMedidaId)).ReturnsAsync(true);
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, fechaVencimientoFutura);

            // 3. Verificar

            // entidad insumo
            Assert.Equal(restauranteId, insumo.RestauranteId);
            Assert.Equal(TipoInsumo.Ingrediente, insumo.Tipo);

            // entidad lote inicial
            Assert.Single(insumo.Lotes);
            Assert.Equal("Lote Harina - 1", insumo.Lotes[0].Nombre);
            Assert.Equal(cantidadInicial, insumo.Lotes[0].Cantidad);

            // se llego a llamar al repo para el alta
            _insumoRepoMock.Verify(r => r.CrearAsync(insumo), Times.Once);
        }
        [Fact]
        public async Task EjecutarAsync_CuandoCategoriaNoExiste_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();

            int categoriaInexistenteId = 99;

            Insumo insumo = new Insumo { CategoriaId = categoriaInexistenteId };

            CategoriaInsumo categoriaVacia = null;

            _categoriaRepoMock.Setup(r => r.ObtenerPorIdAsync(categoriaInexistenteId)).ReturnsAsync(categoriaVacia);

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5)));

            Assert.Equal("La categoría de insumo seleccionada no existe en el sistema.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoUnidadDeMedidaNoExiste_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int unidadDeMedidaInexistenteId = 99;
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = unidadDeMedidaInexistenteId };

            _categoriaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new CategoriaInsumo());
            _unidadMedidaRepoMock.Setup(r => r.ExisteAsync(unidadDeMedidaInexistenteId)).ReturnsAsync(false); // Unidad trucha

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5)));

            Assert.Equal("La unidad de medida seleccionada no existe en el sistema.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoBodegaNoEsDelRestaurante_LanzaArgumentException()
        {
            // 1. Preparar

            int restauranteId = 1;
            int bodegaIdFalsa = 99;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = 1 };

            _categoriaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new CategoriaInsumo());
            _unidadMedidaRepoMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaIdFalsa)).ReturnsAsync(false);

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaIdFalsa, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5)));

            Assert.Equal("La bodega destino especificada no es valida o no existe.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoCantidadInicialEsMenorAlStockMinimo_LanzaArgumentException()
        {
            // 1. Preparar

            int restauranteId = 1;
            int cantidadInicial = 20;
            int cantidadStockMinimo = 50;
            int bodegaId = 1;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = cantidadStockMinimo };

            _categoriaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new CategoriaInsumo());
            _unidadMedidaRepoMock.Setup(r => r.ExisteAsync(bodegaId)).ReturnsAsync(true);
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5)));

            Assert.Equal("La cantidad inicial (20) no puede ser menor al stock mínimo configurado (50).", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoFechaVencimientoEsPasadaOHoy_LanzaArgumentException()
        {
            // 1. Preparar

            int restauranteId = 1;
            int cantidadInicial = 20;
            int bodegaId = 1;
            DateOnly fechaInvalida = DateOnly.FromDateTime(DateTime.UtcNow); // la misma fecha se considera invalida


            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5 };

            _categoriaRepoMock.Setup(r => r.ObtenerPorIdAsync(restauranteId)).ReturnsAsync(new CategoriaInsumo());
            _unidadMedidaRepoMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);


            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, fechaInvalida));

            Assert.Equal("La fecha de vencimiento debe ser una fecha futura.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }
    }
}
