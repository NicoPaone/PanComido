using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ObtenerMesaPorIdCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;

        public ObtenerMesaPorIdCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesaExiste_DevuelveMesa()
        {
            int restauranteId = 1;
            int mesaId = 1;

            var mesasEsperadas = new List<MesaConPosiciones>
            {
                new MesaConPosiciones { Id = mesaId, Numero = 1 },
                new MesaConPosiciones { Id = 2, Numero = 2 }
            };

            _mesaMockRepo
                .Setup(r => r.ObtenerTodasAsync(restauranteId))
                .ReturnsAsync(mesasEsperadas);

            var casoDeUso = new ObtenerMesaPorIdCasoDeUso(_mesaMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(mesaId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(mesaId, resultado.Id);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesaNoExiste_DevuelveNull()
        {
            int restauranteId = 1;
            int mesaId = 99;

            var mesasEsperadas = new List<MesaConPosiciones>
            {
                new MesaConPosiciones { Id = 1, Numero = 1 }
            };

            _mesaMockRepo
                .Setup(r => r.ObtenerTodasAsync(restauranteId))
                .ReturnsAsync(mesasEsperadas);

            var casoDeUso = new ObtenerMesaPorIdCasoDeUso(_mesaMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(mesaId, restauranteId);

            Assert.Null(resultado);
        }
    }
}
