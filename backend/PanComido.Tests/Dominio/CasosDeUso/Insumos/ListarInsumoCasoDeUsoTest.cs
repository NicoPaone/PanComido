using Moq;
using Xunit;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class ListarInsumoCasoDeUsoTests
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IEstadoStockInsumoServicio> _estadoServicioMock;

        public ListarInsumoCasoDeUsoTests()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _estadoServicioMock = new Mock<IEstadoStockInsumoServicio>();
        }

        [Fact]
        public async Task EjecutarAsync_DeberiaObtenerListadoDeInsumoConCantidadTotalFechaDeVencimientoMasProximaYEstadoStock()
        {
            // 1. Preparar

            // Datos de prueba
            int restauranteId = 1;
            int insumoIdFalso = 10;
            decimal stockMinimoFalso = 5m;
            decimal stockActualFalso = 12m;
            var vencimientoFalso = new DateOnly(2026, 10, 15);


            var insumosFalsos = new List<Insumo>
            {
                new Insumo
                {
                    Id = insumoIdFalso,
                    Nombre = "Cebolla",
                    StockMinimo = stockMinimoFalso
                }
            };

            // Configuro los mocks para devolver los datos de prueba
            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(insumosFalsos);


            _loteRepoMock
                .Setup(r => r.ObtenerStockTotalDeInsumo(insumoIdFalso))
                .ReturnsAsync(stockActualFalso);


            _loteRepoMock
                .Setup(r => r.ObtenerFechaDeVencimientoMasProximaDeInsumo(insumoIdFalso))
                .ReturnsAsync(vencimientoFalso);

            _estadoServicioMock
                .Setup(s => s.CalcularEstadoStock(stockActualFalso, stockMinimoFalso, It.IsAny<decimal>()))
                .Returns(EstadoStock.Normal);

            // instancio el caso de uso con sus dependencias mockeadas
            ListarInsumoCasoDeUso casoDeUso = new ListarInsumoCasoDeUso(
                _insumoRepoMock.Object,
                _loteRepoMock.Object,
                _estadoServicioMock.Object
            );

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            _estadoServicioMock.Verify(s => s.CalcularEstadoStock(stockActualFalso, stockMinimoFalso, It.IsAny<decimal>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.Single(resultado);

            var insumoResultante = resultado.First();

            Assert.Equal("Cebolla", insumoResultante.Nombre);
            Assert.Equal(stockActualFalso, insumoResultante.StockActual);
            Assert.Equal(vencimientoFalso, insumoResultante.Vencimiento);
            Assert.Equal(EstadoStock.Normal, insumoResultante.EstadoStock);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayInsumos_DevuelveListaVaciaYNuncaLlamaAlServicio()
        {
            // 1. Preparar
            int restauranteId = 1;
            var listaVacia = new List<Insumo>();

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(listaVacia);

            ListarInsumoCasoDeUso casoDeUso = new ListarInsumoCasoDeUso(
                _insumoRepoMock.Object,
                _loteRepoMock.Object,
                _estadoServicioMock.Object
            );

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            Assert.NotNull(resultado);
            Assert.Empty(resultado);


            // se verifica que si no hubo insumos, nunca se llamo ni al servicio ni al repositorio de lote para obtener su vencimiento o stock total
            _estadoServicioMock.Verify(s =>
                s.CalcularEstadoStock(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Never);
            _loteRepoMock.Verify(r => r.ObtenerStockTotalDeInsumo(It.IsAny<int>()), Times.Never);
            _loteRepoMock.Verify(r => r.ObtenerFechaDeVencimientoMasProximaDeInsumo(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayMultiplesInsumos_ProcesaCadaUnoDeFormaIndependiente()
        {
            // 1. Preparar
            int restauranteId = 1;

            // primer insumo
            int idCebolla = 10;
            decimal minCebolla = 5m;
            decimal stockCebolla = 12m;
            var vtoCebolla = new DateOnly(2026, 10, 15);

            // segundo insumo
            int idTomate = 11;
            decimal minTomate = 2m;
            decimal stockTomate = 1m; // Critico
            var vtoTomate = new DateOnly(2026, 05, 20);

            var insumosFalsos = new List<Insumo>
                {
                    new Insumo { Id = idCebolla, Nombre = "Cebolla", StockMinimo = minCebolla },
                    new Insumo { Id = idTomate, Nombre = "Tomate", StockMinimo = minTomate }
                };

            _insumoRepoMock.Setup(r => r.ObtenerInsumosAsync(restauranteId)).ReturnsAsync(insumosFalsos);

            // que el repo de lote y servicio devuelva data de cebolla
            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumo(idCebolla)).ReturnsAsync(stockCebolla);
            _loteRepoMock.Setup(r => r.ObtenerFechaDeVencimientoMasProximaDeInsumo(idCebolla)).ReturnsAsync(vtoCebolla);
            _estadoServicioMock.Setup(s => s.CalcularEstadoStock(stockCebolla, minCebolla, It.IsAny<decimal>())).Returns(EstadoStock.Normal);

            // que el repo de lote y servicio devuelva data de tomate
            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumo(idTomate)).ReturnsAsync(stockTomate);
            _loteRepoMock.Setup(r => r.ObtenerFechaDeVencimientoMasProximaDeInsumo(idTomate)).ReturnsAsync(vtoTomate);
            _estadoServicioMock.Setup(s => s.CalcularEstadoStock(stockTomate, minTomate, It.IsAny<decimal>())).Returns(EstadoStock.Critico);

            ListarInsumoCasoDeUso casoDeUso = new ListarInsumoCasoDeUso(
                _insumoRepoMock.Object, _loteRepoMock.Object, _estadoServicioMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);

            // cebolla
            var cebollaResultante = resultado.First(i => i.Id == idCebolla);
            Assert.Equal(stockCebolla, cebollaResultante.StockActual);
            Assert.Equal(vtoCebolla, cebollaResultante.Vencimiento);
            Assert.Equal(EstadoStock.Normal, cebollaResultante.EstadoStock);

            // tomate
            var tomateResultante = resultado.First(i => i.Id == idTomate);
            Assert.Equal(stockTomate, tomateResultante.StockActual);
            Assert.Equal(vtoTomate, tomateResultante.Vencimiento);
            Assert.Equal(EstadoStock.Critico, tomateResultante.EstadoStock);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoUnInsumoNoTieneLotes_AsignaCeroEnCantidadNullEnVencimientoYEstadoCritico()
        {
            // 1. Preparar
            int restauranteId = 1;
            int insumoIdFalso = 5;
            decimal stockMinimoFalso = 10m;

            decimal stockVacio = 0m;

            DateOnly? vencimientoVacio = null;

            var insumosFalsos = new List<Insumo>
            {
                new Insumo { Id = insumoIdFalso, Nombre = "Ajo", StockMinimo = stockMinimoFalso }
            };

            _insumoRepoMock.Setup(r => r.ObtenerInsumosAsync(restauranteId)).ReturnsAsync(insumosFalsos);

            // repo lote no encontro lotes de insumo
            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumo(insumoIdFalso)).ReturnsAsync(stockVacio);
            _loteRepoMock.Setup(r => r.ObtenerFechaDeVencimientoMasProximaDeInsumo(insumoIdFalso)).ReturnsAsync(vencimientoVacio);

            
            _estadoServicioMock.Setup(s => s.CalcularEstadoStock(stockVacio, stockMinimoFalso, It.IsAny<decimal>())).Returns(EstadoStock.Critico);

            ListarInsumoCasoDeUso casoDeUso = new ListarInsumoCasoDeUso(
                _insumoRepoMock.Object, _loteRepoMock.Object, _estadoServicioMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            Assert.NotNull(resultado);
            Assert.Single(resultado);

            var ajoResultante = resultado.First();

            // validar situacion de insumo sin lote
            Assert.Equal(0m, ajoResultante.StockActual);
            Assert.Null(ajoResultante.Vencimiento);

            Assert.Equal(EstadoStock.Critico, ajoResultante.EstadoStock);

            // validar que se llamo al servicio con valores de un insumo que no se le pudo encontrar lote, osea 0 de stock
            _estadoServicioMock.Verify(s =>
                s.CalcularEstadoStock(0m, stockMinimoFalso, It.IsAny<decimal>()), Times.Once);
        }
    }
}