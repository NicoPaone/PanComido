using Moq;
using PanComido.Dominio.CasosDeUso.AvisosCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Avisos
{
    public class ListarInsumosConVencimientoProximoCasoDeUsoTest
    {
        [Fact]
        public async Task EjecutarAsync_ObtieneLosInsumosDelRepositorio()
        {
            // Preparar
            int restauranteId = 1;

            var insumos = new List<Insumo>
            {
                new Insumo { Id = 1 }
            };

            var resultadoEsperado = new Dictionary<int, List<Lote>>();

            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var vencimientosServicioMock = new Mock<IVencimientosProximosInsumosServicio>();

            insumoRepoMock
                .Setup(r => r.ObtenerInsumosConLotesAsync(restauranteId))
                .ReturnsAsync(insumos);

            vencimientosServicioMock
                .Setup(s => s.ObtenerVencimientosProximos(insumos, 7))
                .Returns(resultadoEsperado);

            var casoDeUso = new ListarInsumosConVencimientoProximoCasoDeUso(
                insumoRepoMock.Object,
                vencimientosServicioMock.Object);

            // Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            insumoRepoMock.Verify(
                r => r.ObtenerInsumosConLotesAsync(restauranteId),
                Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_LlamaAlServicioDeVencimientosConLosInsumosObtenidos()
        {
            // Preparar
            int restauranteId = 1;

            var insumos = new List<Insumo>
            {
                new Insumo { Id = 1 }
            };

            var resultadoEsperado = new Dictionary<int, List<Lote>>();

            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var vencimientosServicioMock = new Mock<IVencimientosProximosInsumosServicio>();

            insumoRepoMock
                .Setup(r => r.ObtenerInsumosConLotesAsync(restauranteId))
                .ReturnsAsync(insumos);

            vencimientosServicioMock
                .Setup(s => s.ObtenerVencimientosProximos(insumos, 7))
                .Returns(resultadoEsperado);

            var casoDeUso = new ListarInsumosConVencimientoProximoCasoDeUso(
                insumoRepoMock.Object,
                vencimientosServicioMock.Object);

            // Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            vencimientosServicioMock.Verify(
                s => s.ObtenerVencimientosProximos(insumos, 7),
                Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DevuelveElResultadoGeneradoPorElServicio()
        {
            // Preparar
            int restauranteId = 1;

            var insumos = new List<Insumo>
            {
                new Insumo { Id = 1 }
            };

            var resultadoEsperado = new Dictionary<int, List<Lote>>
            {
                {
                    1,
                    new List<Lote>
                    {
                        new Lote { Id = 1 }
                    }
                }
            };

            var insumoRepoMock = new Mock<IInsumoRepositorio>();
            var vencimientosServicioMock = new Mock<IVencimientosProximosInsumosServicio>();

            insumoRepoMock
                .Setup(r => r.ObtenerInsumosConLotesAsync(restauranteId))
                .ReturnsAsync(insumos);

            vencimientosServicioMock
                .Setup(s => s.ObtenerVencimientosProximos(insumos, 7))
                .Returns(resultadoEsperado);

            var casoDeUso = new ListarInsumosConVencimientoProximoCasoDeUso(
                insumoRepoMock.Object,
                vencimientosServicioMock.Object);

            // Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.Same(resultadoEsperado, resultado);
        }
    }
}