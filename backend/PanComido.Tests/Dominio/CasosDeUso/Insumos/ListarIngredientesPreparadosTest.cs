using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class ListarIngredientesPreparadosTest
    {
        private readonly Mock<IIngredientePreparadoRepositorio> _mockRepositorio;
        private readonly Mock<ILoteRepositorio> _mockLoteRepositorio;
        private readonly Mock<IEstadoStockInsumoServicio> _mockEstadoStockServicio;
        private readonly ListarIngredientesPreparados _casoDeUso;

        public ListarIngredientesPreparadosTest()
        {
            _mockRepositorio = new Mock<IIngredientePreparadoRepositorio>();
            _mockLoteRepositorio = new Mock<ILoteRepositorio>();
            _mockEstadoStockServicio = new Mock<IEstadoStockInsumoServicio>();
            
            _casoDeUso = new ListarIngredientesPreparados(
                _mockRepositorio.Object, 
                _mockLoteRepositorio.Object, 
                _mockEstadoStockServicio.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeLlenarDatosDeStockParaCadaIngrediente()
        {
            // Preparar
            int restauranteId = 1;
            var ingrediente = new IngredientePreparado 
            { 
                Id = 10,
                StockMinimo = 5,
                StockRecomendado = 20
            };

            _mockRepositorio.Setup(x => x.ObtenerTodosAsync(restauranteId))
                .ReturnsAsync(new List<IngredientePreparado> { ingrediente });

            _mockLoteRepositorio.Setup(x => x.ObtenerStockTotalDeInsumo(ingrediente.Id))
                .ReturnsAsync(15m);

            var fechaProxima = new DateOnly(2023, 12, 31);
            _mockLoteRepositorio.Setup(x => x.ObtenerFechaDeVencimientoMasProximaDeInsumo(ingrediente.Id))
                .ReturnsAsync(fechaProxima);

            _mockEstadoStockServicio.Setup(x => x.CalcularEstadoStock(15m, 5, 20))
                .Returns(PanComido.Dominio.Entidades.Enums.EstadoStock.Normal);

            // Actuar
            var resultado = await _casoDeUso.Ejecutar(restauranteId);

            // Verificar
            Assert.Single(resultado);
            var resultItem = resultado[0];
            Assert.Equal(15m, resultItem.StockActual);
            Assert.Equal(fechaProxima, resultItem.FechaVencimientoProxima);
            Assert.Equal(PanComido.Dominio.Entidades.Enums.EstadoStock.Normal, resultItem.EstadoStock);

            _mockLoteRepositorio.Verify(x => x.ObtenerStockTotalDeInsumo(ingrediente.Id), Times.Once);
            _mockLoteRepositorio.Verify(x => x.ObtenerFechaDeVencimientoMasProximaDeInsumo(ingrediente.Id), Times.Once);
            _mockEstadoStockServicio.Verify(x => x.CalcularEstadoStock(15m, 5, 20), Times.Once);
        }
    }
}
