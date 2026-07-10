using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ListarMozosParaMesaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;

        public ListarMozosParaMesaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExistenMozos_DevuelveListaDeMozos()
        {
            int restauranteId = 1;

            var mozosEsperados = new List<Empleado>
            {
                new Empleado { Id = 1, Nombre = "Mozo 1", RestauranteId = restauranteId },
                new Empleado { Id = 2, Nombre = "Mozo 2", RestauranteId = restauranteId }
            };

            _mesaMockRepo
                .Setup(r => r.ObtenerTodosLosMozosAsync(restauranteId))
                .ReturnsAsync(mozosEsperados);

            var casoDeUso = new ListarMozosParaMesaCasoDeUso(_mesaMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Mozo 1", resultado[0].Nombre);
            _mesaMockRepo.Verify(r => r.ObtenerTodosLosMozosAsync(restauranteId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayMozos_DevuelveListaVacia()
        {
            int restauranteId = 1;

            _mesaMockRepo
                .Setup(r => r.ObtenerTodosLosMozosAsync(restauranteId))
                .ReturnsAsync(new List<Empleado>());

            var casoDeUso = new ListarMozosParaMesaCasoDeUso(_mesaMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _mesaMockRepo.Verify(r => r.ObtenerTodosLosMozosAsync(restauranteId), Times.Once);
        }
    }
}
