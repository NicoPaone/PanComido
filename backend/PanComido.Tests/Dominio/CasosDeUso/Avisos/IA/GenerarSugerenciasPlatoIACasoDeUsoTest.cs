using Moq;
using PanComido.Dominio.CasosDeUso.AvisosCasosDeUso.IA;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Avisos.IA
{
    public class GenerarSugerenciasPlatoIACasoDeUsoTest
    {
        [Fact]
        public async Task EjecutarAsync_DevuelveSugerenciaExistente_CuandoEsDelDiaActual()
        {
            // Preparar
            int restauranteId = 1;

            var sugerenciaExistente = new SugerenciaIA
            {
                FechaSugerencia = DateTime.Today,
                PlatosSugeridos = new List<PlatoSugeridoIA>
                {
                    new PlatoSugeridoIA { Id = 1, Nombre = "Plato Test" }
                }
            };

            var sugerenciaRepoMock = new Mock<ISugerenciaIARepositorio>();
            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var sugerenciaServicioMock = new Mock<ISugerenciaPlatosIAServicio>();
            var articuloRepoMock = new Mock<IArticuloRepositorio>();
            var vencimientosServicioMock = new Mock<IVencimientosProximosInsumosServicio>();

            sugerenciaRepoMock
                .Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync(sugerenciaExistente);

            var casoDeUso = new GenerarSugerenciasPlatoIACasoDeUso(
                sugerenciaRepoMock.Object,
                insumoRepoMock.Object,
                sugerenciaServicioMock.Object,
                articuloRepoMock.Object,
                vencimientosServicioMock.Object);

            // Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.Same(sugerenciaExistente, resultado);

            sugerenciaServicioMock.Verify(
                s => s.GenerarSugerenciasAsync(
                    It.IsAny<int>(),
                    It.IsAny<List<Insumo>>(),
                    It.IsAny<Dictionary<int, List<Lote>>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<int>()),
                Times.Never);

            sugerenciaRepoMock.Verify(
                r => r.GuardarSugerenciaIAAsync(
                    It.IsAny<int>(),
                    It.IsAny<SugerenciaIA>()),
                Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_GeneraYGuardaUnaNuevaSugerencia()
        {
            // Preparar
            int restauranteId = 1;

            var insumos = new List<Insumo>();

            var vencimientos = new Dictionary<int, List<Lote>>();

            var nuevaSugerencia = new SugerenciaIA();

            var sugerenciaRepoMock = new Mock<ISugerenciaIARepositorio>();
            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var sugerenciaServicioMock = new Mock<ISugerenciaPlatosIAServicio>();
            var articuloRepoMock = new Mock<IArticuloRepositorio>();
            var vencimientosServicioMock = new Mock<IVencimientosProximosInsumosServicio>();

            sugerenciaRepoMock
                .Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync((SugerenciaIA?)null);

            insumoRepoMock
                .Setup(r => r.ObtenerInsumosConLotesAsync(restauranteId))
                .ReturnsAsync(insumos);

            vencimientosServicioMock
                .Setup(v => v.ObtenerVencimientosProximos(insumos, 7))
                .Returns(vencimientos);

            articuloRepoMock
                .Setup(a => a.ObtenerArticulosEnCartaConIngredientesAsync(restauranteId))
                .ReturnsAsync(new List<Articulo>());

            sugerenciaServicioMock
                .Setup(s => s.GenerarSugerenciasAsync(
                    restauranteId,
                    insumos,
                    vencimientos,
                    It.IsAny<List<string>>(),
                    5))
                .ReturnsAsync(nuevaSugerencia);

            var casoDeUso = new GenerarSugerenciasPlatoIACasoDeUso(
                sugerenciaRepoMock.Object,
                insumoRepoMock.Object,
                sugerenciaServicioMock.Object,
                articuloRepoMock.Object,
                vencimientosServicioMock.Object);

            // Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.Same(nuevaSugerencia, resultado);

            sugerenciaServicioMock.Verify(
                s => s.GenerarSugerenciasAsync(
                    restauranteId,
                    insumos,
                    vencimientos,
                    It.IsAny<List<string>>(),
                    5),
                Times.Once);

            sugerenciaRepoMock.Verify(
                r => r.GuardarSugerenciaIAAsync(
                    restauranteId,
                    nuevaSugerencia),
                Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_EnviaLosNombresDeLosPlatosExistentes_ALaIA()
        {
            // Preparar
            int restauranteId = 1;

            var sugerenciaRepoMock = new Mock<ISugerenciaIARepositorio>();
            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var sugerenciaServicioMock = new Mock<ISugerenciaPlatosIAServicio>();
            var articuloRepoMock = new Mock<IArticuloRepositorio>();
            var vencimientosServicioMock = new Mock<IVencimientosProximosInsumosServicio>();

            sugerenciaRepoMock
                .Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync((SugerenciaIA?)null);

            insumoRepoMock
                .Setup(r => r.ObtenerInsumosConLotesAsync(restauranteId))
                .ReturnsAsync(new List<Insumo>());

            vencimientosServicioMock
                .Setup(v => v.ObtenerVencimientosProximos(
                    It.IsAny<List<Insumo>>(),
                    7))
                .Returns(new Dictionary<int, List<Lote>>());

            articuloRepoMock
                .Setup(a => a.ObtenerArticulosEnCartaConIngredientesAsync(restauranteId))
                .ReturnsAsync(new List<Articulo>
                {
            new Plato { Nombre = "Pizza" },
            new Plato { Nombre = "Empanadas" }
                });

            sugerenciaServicioMock
                .Setup(s => s.GenerarSugerenciasAsync(
                    It.IsAny<int>(),
                    It.IsAny<List<Insumo>>(),
                    It.IsAny<Dictionary<int, List<Lote>>>(),
                    It.IsAny<List<string>>(),
                    5))
                .ReturnsAsync(new SugerenciaIA());

            var casoDeUso = new GenerarSugerenciasPlatoIACasoDeUso(
                sugerenciaRepoMock.Object,
                insumoRepoMock.Object,
                sugerenciaServicioMock.Object,
                articuloRepoMock.Object,
                vencimientosServicioMock.Object);

            // Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            sugerenciaServicioMock.Verify(
                s => s.GenerarSugerenciasAsync(
                    restauranteId,
                    It.IsAny<List<Insumo>>(),
                    It.IsAny<Dictionary<int, List<Lote>>>(),
                    It.Is<List<string>>(nombres =>
                        nombres.Count == 2 &&
                        nombres.Contains("Pizza") &&
                        nombres.Contains("Empanadas")),
                    5),
                Times.Once);
        }
    }
}
