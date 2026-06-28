using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerAnalisisPlatoCasoDeUsoTest
    {
        private readonly Mock<IPlatoAnalisisRepositorio> _platoAnalisisRepoMock;
        private readonly Mock<ISugerenciaIARepositorio> _sugerenciaIaRepoMock;
        private readonly Mock<ICalculadorCostoPlatoServicio> _calculadorCostoMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly ObtenerAnalisisPlatoCasoDeUso _casoDeUso;

        public ObtenerAnalisisPlatoCasoDeUsoTest()
        {
            _platoAnalisisRepoMock = new Mock<IPlatoAnalisisRepositorio>();
            _sugerenciaIaRepoMock = new Mock<ISugerenciaIARepositorio>();
            _calculadorCostoMock = new Mock<ICalculadorCostoPlatoServicio>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _casoDeUso = new ObtenerAnalisisPlatoCasoDeUso(
                _platoAnalisisRepoMock.Object, 
                _sugerenciaIaRepoMock.Object,
                _calculadorCostoMock.Object,
                _dateTimeProviderMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarNull_CuandoNoExistePlato()
        {
            int restauranteId = 1;
            string nombrePlato = "NoExiste";
            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync((Articulo?)null);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCalcularMetricasYSugerenciasCorrectamente()
        {
            int restauranteId = 1;
            string nombrePlato = "Papas Fritas";
            var plato = new Plato
            {
                Id = 10,
                Nombre = nombrePlato,
                PrecioVentaFinal = 4000,
                CategoriaPlatoId = 2,
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente { InsumoId = 101, Cantidad = 0.5m }
                }
            };

            var fechaReferencia = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaReferencia);
            _dateTimeProviderMock.Setup(d => d.ObtenerHoy()).Returns(fechaReferencia.Date);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync(plato);

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato))
                .ReturnsAsync(1000m);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(15);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasCategoriaEnRangoAsync(restauranteId, plato.CategoriaPlatoId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(100);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerPlatoLiderDeCategoriaAsync(restauranteId, plato.CategoriaPlatoId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new RendimientoPlato { PlatoId = 20, Nombre = "Papas Rusticas", UnidadesVendidas = 45, FacturacionTotal = 202500m });

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasSemanalesArticuloAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<int> { 0, 1, 2, 3, 4, 5, 6 });

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync((SugerenciaIA?)null);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.NotNull(resultado);
            Assert.Equal(plato, resultado.Articulo);
            Assert.Equal(1000m, resultado.CostoPreparacion);
            Assert.Equal(15, resultado.VentasPeriodo);
            Assert.Equal("15.0%", resultado.Participacion);
            Assert.Equal("Papas Rusticas", resultado.ComparativaLider.Nombre);
            Assert.Equal(7, resultado.Tendencia.Count);
            Assert.NotNull(resultado.AnalisisIa);
            Assert.Equal(2, resultado.AnalisisIa.Sugerencias.Count);
            
            _sugerenciaIaRepoMock.Verify(r => r.GuardarSugerenciaIAAsync(restauranteId, It.IsAny<SugerenciaIA>()), Times.Once);
        }
    }
}
