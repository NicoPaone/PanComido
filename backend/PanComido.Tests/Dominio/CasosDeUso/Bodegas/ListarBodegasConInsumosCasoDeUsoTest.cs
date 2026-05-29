using Moq;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.Bodegas
{
    public class ListarBodegasConInsumosCasoDeUsoTests
    {
        private readonly Mock<IBodegaRepositorio> _bodegaRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<IEstadoStockInsumoServicio> _estadoServicioMock;

        public ListarBodegasConInsumosCasoDeUsoTests()
        {
            _bodegaRepoMock = new Mock<IBodegaRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _estadoServicioMock = new Mock<IEstadoStockInsumoServicio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayDatos_CruzaBodegasEInsumosCorrectamente()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 1;
            int insumoId = 10;
            decimal stockActual = 15m;
            decimal stockMinimo = 5m;
            var vencimiento = new DateOnly(2026, 12, 1);

            var bodegasFalsas = new List<Bodega> { new Bodega { Id = bodegaId, Nombre = "Heladera" } };
            var insumosFalsos = new List<Insumo> { new Insumo { Id = insumoId, Nombre = "Tomate", StockMinimo = stockMinimo } };

            var stocksMap = new Dictionary<(int, int), decimal> { { (insumoId, bodegaId), stockActual } };
            var vencimientosMap = new Dictionary<(int, int), DateOnly?> { { (insumoId, bodegaId), vencimiento } };

            _bodegaRepoMock.Setup(r => r.ObtenerBodegasAsync(restauranteId)).ReturnsAsync(bodegasFalsas);
            _insumoRepoMock.Setup(r => r.ObtenerInsumosAsync(restauranteId)).ReturnsAsync(insumosFalsos);

            _loteRepoMock.Setup(r => r.ObtenerStocksPorBodega(restauranteId)).ReturnsAsync(stocksMap);
            _loteRepoMock.Setup(r => r.ObtenerVencimientosPorBodega(restauranteId)).ReturnsAsync(vencimientosMap);

            _estadoServicioMock.Setup(s => s.CalcularEstadoStock(stockActual, stockMinimo)).Returns(EstadoStock.Normal);

            var casoDeUso = new ListarBodegasConInsumosCasoDeUso(_bodegaRepoMock.Object, _loteRepoMock.Object, _insumoRepoMock.Object, _estadoServicioMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            Assert.Single(resultado);
            var bodegaResultante = resultado[0];
            Assert.Equal("Heladera", bodegaResultante.Nombre);

            Assert.Single(bodegaResultante.Insumos);
            var insumoResultante = bodegaResultante.Insumos[0];

            Assert.Equal(insumoId, insumoResultante.Id);
            Assert.Equal(stockActual, insumoResultante.StockActual);
            Assert.Equal(vencimiento, insumoResultante.Vencimiento);
            Assert.Equal(EstadoStock.Normal, insumoResultante.EstadoStock);

            _estadoServicioMock.Verify(s => s.CalcularEstadoStock(stockActual, stockMinimo), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoInsumoNoEstaEnLaBodega_NoLoAgregaALaLista()
        {
            // 1. Preparar
            int restauranteId = 1;

            var bodegasFalsas = new List<Bodega> { new Bodega { Id = 1, Nombre = "Heladera" } };
            var insumosFalsos = new List<Insumo>
            {
                new Insumo { Id = 10, Nombre = "Tomate" }, // este si va a estar
                new Insumo { Id = 11, Nombre = "Carne" }   // este NO va a estar
            };

            // solo meto el tomate en la bodega de Heladera, la carne simplemente esta para ver si la filtra
            var stocksMap = new Dictionary<(int, int), decimal> { { (10, 1), 15m } };
            var vencimientosMap = new Dictionary<(int, int), DateOnly?> { { (10, 1), new DateOnly(2026, 12, 1) } };

            _bodegaRepoMock.Setup(r => r.ObtenerBodegasAsync(restauranteId)).ReturnsAsync(bodegasFalsas);
            _insumoRepoMock.Setup(r => r.ObtenerInsumosAsync(restauranteId)).ReturnsAsync(insumosFalsos);
            _loteRepoMock.Setup(r => r.ObtenerStocksPorBodega(restauranteId)).ReturnsAsync(stocksMap);
            _loteRepoMock.Setup(r => r.ObtenerVencimientosPorBodega(restauranteId)).ReturnsAsync(vencimientosMap);

            var casoDeUso = new ListarBodegasConInsumosCasoDeUso(_bodegaRepoMock.Object, _loteRepoMock.Object, _insumoRepoMock.Object, _estadoServicioMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            var bodega = resultado.First();

            Assert.Single(bodega.Insumos); 
            Assert.Equal(10, bodega.Insumos[0].Id); // solo llego tomate (id 10)
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayBodegas_DevuelveListaVacia_Y_NoLlamaAlServicio()
        {
            // 1. Preparar
            int restauranteId = 1;

            _bodegaRepoMock.Setup(r => r.ObtenerBodegasAsync(restauranteId)).ReturnsAsync(new List<Bodega>());
            _insumoRepoMock.Setup(r => r.ObtenerInsumosAsync(restauranteId)).ReturnsAsync(new List<Insumo>());
            _loteRepoMock.Setup(r => r.ObtenerStocksPorBodega(restauranteId)).ReturnsAsync(new Dictionary<(int, int), decimal>());
            _loteRepoMock.Setup(r => r.ObtenerVencimientosPorBodega(restauranteId)).ReturnsAsync(new Dictionary<(int, int), DateOnly?>());

            var casoDeUso = new ListarBodegasConInsumosCasoDeUso(_bodegaRepoMock.Object, _loteRepoMock.Object, _insumoRepoMock.Object, _estadoServicioMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _estadoServicioMock.Verify(s => s.CalcularEstadoStock(It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoUnInsumoEstaEnVariasBodegas_MantieneDatosIndependientes()
        {
            // 1. Preparar
            int restauranteId = 1;
            int insumoId = 10;

            
            var bodegas = new List<Bodega> 
            {
                new Bodega { Id = 1 },
                new Bodega { Id = 2 } 
            };

            var insumos = new List<Insumo> { new Insumo { Id = insumoId, StockMinimo = 5m } };

            // insumo id 10 va a estar en 2 bodegas distintas (un lote en cada bodega)
            var stocksMap = new Dictionary<(int, int), decimal> 
            { 
                { (insumoId, 1), 15m },
                { (insumoId, 2), 3m } 
            };
            var vencimientosMap = new Dictionary<(int, int), DateOnly?> { { (insumoId, 1), new DateOnly(2026, 12, 1) }, { (insumoId, 2), new DateOnly(2025, 01, 01) } };

            _bodegaRepoMock.Setup(r => r.ObtenerBodegasAsync(restauranteId)).ReturnsAsync(bodegas);
            _insumoRepoMock.Setup(r => r.ObtenerInsumosAsync(restauranteId)).ReturnsAsync(insumos);
            _loteRepoMock.Setup(r => r.ObtenerStocksPorBodega(restauranteId)).ReturnsAsync(stocksMap);
            _loteRepoMock.Setup(r => r.ObtenerVencimientosPorBodega(restauranteId)).ReturnsAsync(vencimientosMap); // Ajustar nombre variable

            var casoDeUso = new ListarBodegasConInsumosCasoDeUso(_bodegaRepoMock.Object, _loteRepoMock.Object, _insumoRepoMock.Object, _estadoServicioMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            var heladera = resultado.First(b => b.Id == 1);
            var almacen = resultado.First(b => b.Id == 2);


            Assert.Equal(15m, heladera.Insumos[0].StockActual); // heladera tiene 15 de el insumo id 10
            Assert.Equal(3m, almacen.Insumos[0].StockActual); // almacen tiene 3 del mismo insumo id 10
        }

        [Fact]
        public async Task EjecutarAsync_CuandoVencimientoEsNull_MapeaNullCorrectamente()
        {
            // 1. Preparar
            int restauranteId = 1;

            var bodegasFalsas = new List<Bodega> { new Bodega { Id = 1 } };
            var insumosFalsos = new List<Insumo> { new Insumo { Id = 10, StockMinimo = 5m } };

            var stocksMap = new Dictionary<(int, int), decimal> { { (10, 1), 0m } }; // Stock 0

            // Mandamos NULL en el diccionario de vencimientos
            var vencimientosMap = new Dictionary<(int, int), DateOnly?> { { (10, 1), null } };

            _bodegaRepoMock.Setup(r => r.ObtenerBodegasAsync(restauranteId)).ReturnsAsync(bodegasFalsas);
            _insumoRepoMock.Setup(r => r.ObtenerInsumosAsync(restauranteId)).ReturnsAsync(insumosFalsos);
            _loteRepoMock.Setup(r => r.ObtenerStocksPorBodega(restauranteId)).ReturnsAsync(stocksMap);
            _loteRepoMock.Setup(r => r.ObtenerVencimientosPorBodega(restauranteId)).ReturnsAsync(vencimientosMap);

            _estadoServicioMock.Setup(s => s.CalcularEstadoStock(0m, 5m)).Returns(EstadoStock.Critico);

            var casoDeUso = new ListarBodegasConInsumosCasoDeUso(_bodegaRepoMock.Object, _loteRepoMock.Object, _insumoRepoMock.Object, _estadoServicioMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            var insumo = resultado[0].Insumos[0];
            Assert.Equal(0m, insumo.StockActual);
            Assert.Null(insumo.Vencimiento);
            Assert.Equal(EstadoStock.Critico, insumo.EstadoStock);
        }
    }
}