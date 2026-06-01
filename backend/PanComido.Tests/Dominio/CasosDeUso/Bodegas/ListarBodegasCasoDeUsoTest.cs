using Moq;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Bodegas
{
    public class ListarBodegasCasoDeUsoTest
    {
        [Fact]
        public async Task EjecutarAsync_DevuelveListaDeBodegas_Y_LlamaAlRepositorio()
        {
            // 1. Preparar
            int restauranteId = 1;
            var bodegasFalsas = new List<Bodega>
            {
                new Bodega { Id = 1, Nombre = "Heladera Principal" }
            };

            var bodegaRepoMock = new Mock<IBodegaRepositorio>();
            bodegaRepoMock.Setup(r => r.ObtenerBodegasAsync(restauranteId)).ReturnsAsync(bodegasFalsas);

            var casoDeUso = new ListarBodegasCasoDeUso(bodegaRepoMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            bodegaRepoMock.Verify(r => r.ObtenerBodegasAsync(restauranteId), Times.Once);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("Heladera Principal", resultado[0].Nombre);
        }
    }
}
