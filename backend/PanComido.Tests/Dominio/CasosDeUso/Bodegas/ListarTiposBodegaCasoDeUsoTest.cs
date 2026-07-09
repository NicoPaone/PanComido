using Moq;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Bodegas
{
    public class ListarTiposBodegaCasoDeUsoTest
    {
        private readonly Mock<ITipoBodegaRepositorio> _repoMock;
        private readonly ListarTiposBodegaCasoDeUso _casoDeUso;

        public ListarTiposBodegaCasoDeUsoTest()
        {
            _repoMock = new Mock<ITipoBodegaRepositorio>();
            _casoDeUso = new ListarTiposBodegaCasoDeUso(_repoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_RetornaListaDeTiposBodega()
        {
            // Preparar
            var tiposMock = new List<TipoBodega>
            {
                new TipoBodega { Id = 1, Descripcion = "Almacén" },
                new TipoBodega { Id = 2, Descripcion = "Cámara Frigorífica" }
            };

            _repoMock
                .Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(tiposMock);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync();

            // Validar
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Almacén", resultado[0].Descripcion);
            _repoMock.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }
    }
}
