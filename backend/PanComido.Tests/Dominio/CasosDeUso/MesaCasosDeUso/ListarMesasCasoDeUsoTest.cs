using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ListarMesasCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;

        public ListarMesasCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_DevuelveListaDeMesas()
        {
            int restauranteId = 1;

            var mesasEsperadas = new List<MesaConPosiciones>
            {
                new MesaConPosiciones { Id = 1, Numero = 1 },
                new MesaConPosiciones { Id = 2, Numero = 2 }
            };

            _mesaMockRepo
                .Setup(r => r.ObtenerTodasAsync(restauranteId))
                .ReturnsAsync(mesasEsperadas);

            var casoDeUso = new ListarMesasCasoDeUso(_mesaMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal(1, resultado[0].Numero);
            _mesaMockRepo.Verify(r => r.ObtenerTodasAsync(restauranteId), Times.Once());
        }
    }
}
