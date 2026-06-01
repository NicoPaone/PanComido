using Moq;
using PanComido.Dominio.CasosDeUso.UnidadMedidaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.UnidadDeMedida
{
    public class ListarUnidadesDeMedidaCasoDeUsoTest
    {

        [Fact]
        public async Task EjecutarAsync_DevuelveListaDeUnidadesDeMedida_Y_LlamaAlRepositorioCorrespondiente()
        {
            // 1. Preparar
            int restauranteId = 1;
            var unidadesFalsas = new List<UnidadMedida>
            {
                new UnidadMedida { Id = 1, Nombre = "KG" }
            };

            var unidadesDeMedidaRepoMock = new Mock<IUnidadMedidaRepositorio>();
            unidadesDeMedidaRepoMock.Setup(r => r.ObtenerUnidadesDeMedidaAsync()).ReturnsAsync(unidadesFalsas);

            var casoDeUso = new ListarUnidadesDeMedidaCasoDeUso(unidadesDeMedidaRepoMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync();

            // 3. Verificar
            unidadesDeMedidaRepoMock.Verify(r => r.ObtenerUnidadesDeMedidaAsync(), Times.Once);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("KG", resultado[0].Nombre);
        }

    }
}
