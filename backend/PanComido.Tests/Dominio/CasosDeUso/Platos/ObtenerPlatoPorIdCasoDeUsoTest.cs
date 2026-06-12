using Moq;
using PanComido.Dominio.CasosDeUso.PlatoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Platos
{
    public class ObtenerPlatoPorIdCasoDeUsoTest
    {
        private readonly Mock<IPlatoRepositorio> _platoRepoMock;
        private readonly ObtenerPlatoPorIdCasoDeUso _casoDeUso;

        public ObtenerPlatoPorIdCasoDeUsoTest()
        {
            _platoRepoMock = new Mock<IPlatoRepositorio>();
            _casoDeUso = new ObtenerPlatoPorIdCasoDeUso(_platoRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoPlatoExiste_RetornaPlato()
        {
            // Preparar
            int platoId = 5;
            int restauranteId = 1;
            var platoEsperado = new Plato { Id = platoId, Nombre = "Ravioles" };

            // Simulamos que encontramos el plato
            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoId, restauranteId))
                          .ReturnsAsync(platoEsperado);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(platoId, restauranteId);

            // Verificar
            Assert.NotNull(resultado);
            Assert.Equal(platoId, resultado.Id);
            Assert.Equal("Ravioles", resultado.Nombre);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoPlatoNoExiste_RetornaNull()
        {
            // Preparar
            int platoId = 99;
            int restauranteId = 1;

            // Simulamos que la BD no encuentra el plato
            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoId, restauranteId))
                          .ReturnsAsync((Plato)null);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(platoId, restauranteId);

            // Verificar
            Assert.Null(resultado);
        }
    }
}
