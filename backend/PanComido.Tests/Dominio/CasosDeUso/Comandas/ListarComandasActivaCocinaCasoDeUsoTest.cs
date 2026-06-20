using Moq;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Comandas
{
    public class ListarComandasActivaCocinaCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaRepositorioMock;
        private readonly ListarComandaActivaCocinaCasoDeUso _casoDeUso;

        public ListarComandasActivaCocinaCasoDeUsoTest()
        {
            _comandaRepositorioMock = new Mock<IComandaRepositorio>();
            _casoDeUso = new ListarComandaActivaCocinaCasoDeUso(_comandaRepositorioMock.Object);
        }

        [Fact]
        public async Task Ejecutar_DevuelveLasComandasActivasParaCocina()
        {
            // Preparar
            var comandas = new List<Comanda>
            {
                new Comanda { Id = 1 },
                new Comanda { Id = 2 }
            };

            _comandaRepositorioMock
                .Setup(r => r.ObtenerComandasActivasParaCocinaAsync(1))
                .ReturnsAsync(comandas);

            // Ejecutar
            var resultado = await _casoDeUso.Ejecutar(1);

            // Verificar
            Assert.Equal(2, resultado.Count);
            Assert.Equal(1, resultado[0].Id);
            Assert.Equal(2, resultado[1].Id);

            _comandaRepositorioMock.Verify(
                r => r.ObtenerComandasActivasParaCocinaAsync(1),
                Times.Once);
        }
    }
}
