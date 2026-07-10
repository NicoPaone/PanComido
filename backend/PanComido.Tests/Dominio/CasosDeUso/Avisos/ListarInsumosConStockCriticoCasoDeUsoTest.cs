using Moq;
using PanComido.Dominio.CasosDeUso.AvisosCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Avisos
{
    public class ListarInsumosConStockCriticoCasoDeUsoTest
    {
        [Fact]
        public async Task EjecutarAsync_DevuelveSoloLosInsumosConStockCritico()
        {
            // 1. Preparar
            int restauranteId = 1;

            var insumoCritico = new Insumo
            {
                Id = 1,
                Nombre = "Harina",
                StockMinimo = 10
            };

            var insumoNormal = new Insumo
            {
                Id = 2,
                Nombre = "Azucar",
                StockMinimo = 10
            };

            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var loteRepoMock = new Mock<ILoteRepositorio>();
            var estadoStockMock = new Mock<IEstadoStockInsumoServicio>();

            insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(new List<Insumo> { insumoCritico, insumoNormal });

            loteRepoMock
                .Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>()))
                .ReturnsAsync(new Dictionary<int, decimal> { { 1, 5 }, { 2, 20 } });

            loteRepoMock
                .Setup(r => r.ObtenerFechaDeVencimientoMasProximaDeInsumo(It.IsAny<int>()))
                .ReturnsAsync(DateOnly.FromDateTime(DateTime.Today.AddDays(10)));

            estadoStockMock
                .Setup(s => s.CalcularEstadoStock(5, 10, It.IsAny<decimal>()))
                .Returns(EstadoStock.Critico);

            estadoStockMock
                .Setup(s => s.CalcularEstadoStock(20, 10, It.IsAny<decimal>()))
                .Returns(EstadoStock.Normal);

            var casoDeUso = new ListarInsumosConStockCriticoCasoDeUso(
                insumoRepoMock.Object,
                estadoStockMock.Object,
                loteRepoMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            insumoRepoMock.Verify(r => r.ObtenerInsumosAsync(restauranteId), Times.Once);

            Assert.Single(resultado);
            Assert.Equal("Harina", resultado[0].Nombre);
            Assert.Equal(EstadoStock.Critico, resultado[0].EstadoStock);
        }

        [Fact]
        public async Task EjecutarAsync_DevuelveListaVacia_CuandoNoHayInsumosCriticos()
        {
            // Preparar
            int restauranteId = 1;

            var insumo = new Insumo
            {
                Id = 1,
                Nombre = "Harina",
                StockMinimo = 10
            };

            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var loteRepoMock = new Mock<ILoteRepositorio>();
            var estadoStockMock = new Mock<IEstadoStockInsumoServicio>();

            insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(new List<Insumo> { insumo });

            loteRepoMock
                .Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>()))
                .ReturnsAsync(new Dictionary<int, decimal> { { insumo.Id, 20 } });

            loteRepoMock
                .Setup(r => r.ObtenerFechaDeVencimientoMasProximaDeInsumo(insumo.Id))
                .ReturnsAsync(DateOnly.FromDateTime(DateTime.Today.AddDays(15)));

            estadoStockMock
                .Setup(s => s.CalcularEstadoStock(20, 10, It.IsAny<decimal>()))
                .Returns(EstadoStock.Normal);

            var casoDeUso = new ListarInsumosConStockCriticoCasoDeUso(
                insumoRepoMock.Object,
                estadoStockMock.Object,
                loteRepoMock.Object);

            // Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.Empty(resultado);

            insumoRepoMock.Verify(
                r => r.ObtenerInsumosAsync(restauranteId),
                Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_ActualizaStockYVencimientoDelInsumo()
        {
            // Preparar
            int restauranteId = 1;

            var fechaVencimiento = DateOnly.FromDateTime(
                DateTime.Today.AddDays(5));

            var insumo = new Insumo
            {
                Id = 1,
                Nombre = "Harina",
                StockMinimo = 10
            };

            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var loteRepoMock = new Mock<ILoteRepositorio>();
            var estadoStockMock = new Mock<IEstadoStockInsumoServicio>();

            insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(new List<Insumo> { insumo });

            loteRepoMock
                .Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>()))
                .ReturnsAsync(new Dictionary<int, decimal> { { insumo.Id, 3 } });

            loteRepoMock
                .Setup(r => r.ObtenerFechaDeVencimientoMasProximaDeInsumo(insumo.Id))
                .ReturnsAsync(fechaVencimiento);

            estadoStockMock
                .Setup(s => s.CalcularEstadoStock(3, 10, It.IsAny<decimal>()))
                .Returns(EstadoStock.Critico);

            var casoDeUso = new ListarInsumosConStockCriticoCasoDeUso(
                insumoRepoMock.Object,
                estadoStockMock.Object,
                loteRepoMock.Object);

            // Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.Single(resultado);

            Assert.Equal(3, resultado[0].StockActual);
            Assert.Equal(fechaVencimiento, resultado[0].Vencimiento);
            Assert.Equal(EstadoStock.Critico, resultado[0].EstadoStock);
        }
    }
}
