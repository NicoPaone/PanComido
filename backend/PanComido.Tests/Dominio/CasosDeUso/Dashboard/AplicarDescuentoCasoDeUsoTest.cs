using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Dashboard
{
    public class AplicarDescuentoCasoDeUsoTest
    {
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly Mock<ISugerenciaIARepositorio> _sugerenciaIaRepoMock;
        private readonly Mock<IPlatoAnalisisRepositorio> _platoAnalisisRepoMock;
        private readonly AplicarDescuentoCasoDeUso _casoDeUso;

        public AplicarDescuentoCasoDeUsoTest()
        {
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _sugerenciaIaRepoMock = new Mock<ISugerenciaIARepositorio>();
            _platoAnalisisRepoMock = new Mock<IPlatoAnalisisRepositorio>();
            _casoDeUso = new AplicarDescuentoCasoDeUso(
                _articuloRepoMock.Object,
                _sugerenciaIaRepoMock.Object,
                _platoAnalisisRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeAplicarDescuentoActualizarBaseDeDatosYTogglearSugerencia()
        {
            // Preparar
            int restauranteId = 1;
            int platoId = 10;
            decimal porcentajeDescuento = 10m;
            var plato = new Plato
            {
                Id = platoId,
                Nombre = "Papas Fritas",
                PrecioVentaFinal = 4000,
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente { InsumoId = 101, Cantidad = 0.5m }
                }
            };

            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, platoId))
                .ReturnsAsync(plato);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerUltimoPrecioCompraInsumoAsync(101))
                .ReturnsAsync(2000m); // Costo: 1000m

            var sugerenciaIa = new SugerenciaIA
            {
                PlatosAnalisis = new List<PlatoAnalisisIa>
                {
                    new PlatoAnalisisIa
                    {
                        PlatoId = platoId,
                        Sugerencias = new List<PlatoSugerenciaIa>
                        {
                            new PlatoSugerenciaIa { Id = 1, Tipo = "descuento", Accion = "AccionDescuento", Aplicada = false }
                        }
                    }
                }
            };

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync(sugerenciaIa);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, platoId, porcentajeDescuento);

            // Verificar
            Assert.NotNull(resultado);
            Assert.Equal(3600m, resultado.PrecioNuevo); // 4000 - 10% = 3600
            Assert.Equal(1000m, resultado.Costo);
            Assert.Equal("72%", resultado.MargenPctNuevo); // (3600 - 1000) / 3600 * 100 = 72.22%
            Assert.True(sugerenciaIa.PlatosAnalisis[0].Sugerencias[0].Aplicada);

            _articuloRepoMock.Verify(r => r.ActualizarAsync(It.Is<Articulo>(a => a.PrecioVentaFinal == 3600m)), Times.Once);
            _sugerenciaIaRepoMock.Verify(r => r.GuardarSugerenciaIAAsync(restauranteId, sugerenciaIa), Times.Once);
        }
    }
}
