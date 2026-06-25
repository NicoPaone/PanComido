using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
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
        private readonly ObtenerAnalisisPlatoCasoDeUso _casoDeUso;

        public ObtenerAnalisisPlatoCasoDeUsoTest()
        {
            _platoAnalisisRepoMock = new Mock<IPlatoAnalisisRepositorio>();
            _sugerenciaIaRepoMock = new Mock<ISugerenciaIARepositorio>();
            _casoDeUso = new ObtenerAnalisisPlatoCasoDeUso(_platoAnalisisRepoMock.Object, _sugerenciaIaRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarNull_CuandoNoExistePlato()
        {
            // Preparar
            int restauranteId = 1;
            string nombrePlato = "NoExiste";
            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync((Articulo?)null);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            // Verificar
            Assert.Null(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCalcularMetricasYSugerenciasCorrectamente()
        {
            // Preparar
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

            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync(plato);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerUltimoPrecioCompraInsumoAsync(101))
                .ReturnsAsync(2000m); // Costo: 0.5 * 2000 = 1000m

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(15);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasCategoriaEnRangoAsync(restauranteId, plato.CategoriaPlatoId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(100);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerPlatoLiderDeCategoriaAsync(restauranteId, plato.CategoriaPlatoId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new RendimientoPlato { PlatoId = 20, Nombre = "Papas Rusticas", UnidadesVendidas = 45, FacturacionTotal = 202500m });

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync((SugerenciaIA?)null);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            // Verificar
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
