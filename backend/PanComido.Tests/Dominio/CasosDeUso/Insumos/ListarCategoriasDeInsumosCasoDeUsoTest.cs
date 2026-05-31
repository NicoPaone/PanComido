using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class ListarCategoriasDeInsumosCasoDeUsoTest
    {
        [Fact]
        public async Task EjecutarAsync_DevuelveListaDeCategoriasDeInsumo_Y_LlamaAlRepositorioCorrespondiente()
        {
            // 1. Preparar
            int restauranteId = 1;
            var categoriasFalsas = new List<CategoriaInsumo>
            {
                new CategoriaInsumo { Id = 1, Descripcion = "Verduras" }
            };

            var categoriaInsumoRepoMock = new Mock<ICategoriaInsumoRepositorio>();
            categoriaInsumoRepoMock.Setup(r => r.ObtenerCategoriasInsumoAsync()).ReturnsAsync(categoriasFalsas);

            var casoDeUso = new ListarCategoriasDeInsumosCasoDeUso(categoriaInsumoRepoMock.Object);

            // 2. Ejecutar
            var resultado = await casoDeUso.EjecutarAsync();

            // 3. Verificar
            categoriaInsumoRepoMock.Verify(r => r.ObtenerCategoriasInsumoAsync(), Times.Once);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("Verduras", resultado[0].Descripcion);
        }
    }
}
