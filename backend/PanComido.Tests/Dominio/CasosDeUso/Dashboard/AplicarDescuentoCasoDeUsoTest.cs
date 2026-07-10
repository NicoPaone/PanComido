using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Dashboard;
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
    public class AplicarDescuentoCasoDeUsoTest
    {
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly Mock<ISugerenciaIARepositorio> _sugerenciaIaRepoMock;
        private readonly Mock<ICalculadorCostoPlatoServicio> _calculadorCostoMock;
        private readonly Mock<IPoliticaDescuentoDashboardServicio> _politicaDescuentoMock;
        private readonly Mock<ITransaccionPersistenciaServicio> _transaccionMock;
        private readonly AplicarDescuentoCasoDeUso _casoDeUso;

        public AplicarDescuentoCasoDeUsoTest()
        {
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _sugerenciaIaRepoMock = new Mock<ISugerenciaIARepositorio>();
            _calculadorCostoMock = new Mock<ICalculadorCostoPlatoServicio>();
            _politicaDescuentoMock = new Mock<IPoliticaDescuentoDashboardServicio>();
            _transaccionMock = new Mock<ITransaccionPersistenciaServicio>();

            _politicaDescuentoMock.Setup(p => p.ObtenerAsync(It.IsAny<int>()))
                .ReturnsAsync(new PoliticaDescuentoDashboard
                {
                    PorcentajeDescuentoMaximo = 80m,
                    MargenMinimoPermitido = 20m
                });

            _transaccionMock.Setup(t => t.EjecutarAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(operacion => operacion());

            _casoDeUso = new AplicarDescuentoCasoDeUso(
                _articuloRepoMock.Object,
                _sugerenciaIaRepoMock.Object,
                _calculadorCostoMock.Object,
                _politicaDescuentoMock.Object,
                _transaccionMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeAplicarDescuentoActualizarBaseDeDatosYTogglearSugerencia()
        {
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

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato))
                .ReturnsAsync(1000m);

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

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, platoId, porcentajeDescuento);

            Assert.NotNull(resultado);
            Assert.Equal(3600m, resultado.PrecioNuevo);
            Assert.Equal(1000m, resultado.Costo);
            Assert.Equal("72%", resultado.MargenPctNuevo);
            Assert.True(sugerenciaIa.PlatosAnalisis[0].Sugerencias[0].Aplicada);

            _articuloRepoMock.Verify(r => r.ActualizarAsync(It.Is<Articulo>(a => a.PrecioVentaFinal == 3600m)), Times.Once);
            _sugerenciaIaRepoMock.Verify(r => r.GuardarSugerenciaIAAsync(restauranteId, sugerenciaIa), Times.Once);
            _transaccionMock.Verify(t => t.EjecutarAsync(It.IsAny<Func<Task>>()), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(81)]
        public async Task EjecutarAsync_DebeRechazarDescuentoFueraDeRango(decimal porcentajeDescuento)
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, 10, porcentajeDescuento));

            _articuloRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Articulo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRechazarDescuentoQueRompeMargenMinimo()
        {
            int restauranteId = 1;
            int platoId = 10;
            var plato = new Plato
            {
                Id = platoId,
                Nombre = "Papas Fritas",
                PrecioVentaFinal = 4000
            };

            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, platoId))
                .ReturnsAsync(plato);

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato))
                .ReturnsAsync(3000m);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, platoId, 10m));

            _articuloRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Articulo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeUsarPoliticaConfiguradaParaValidarDescuentoMaximo()
        {
            _politicaDescuentoMock.Setup(p => p.ObtenerAsync(1))
                .ReturnsAsync(new PoliticaDescuentoDashboard
                {
                    PorcentajeDescuentoMaximo = 30m,
                    MargenMinimoPermitido = 20m
                });

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, 10, 31m));

            Assert.Contains("30", ex.Message);
            _articuloRepoMock.Verify(r => r.ObtenerDetalleAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }
}
