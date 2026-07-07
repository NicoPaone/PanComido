using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.EmpleadoCasosDeUso
{
    public class ListarEmpleadosCasoDeUsoTest
    {
        private readonly Mock<IEmpleadoRepositorio> _repoMock;
        private readonly ListarEmpleadosCasoDeUso _casoDeUso;

        public ListarEmpleadosCasoDeUsoTest()
        {
            _repoMock = new Mock<IEmpleadoRepositorio>();
            _casoDeUso = new ListarEmpleadosCasoDeUso(_repoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExistenEmpleados_DevuelveLista()
        {
            int restauranteId = 1;
            var empleadosEsperados = new List<Empleado>
            {
                new Empleado { Id = 1, Nombre = "Juan", RestauranteId = restauranteId },
                new Empleado { Id = 2, Nombre = "Maria", RestauranteId = restauranteId }
            };

            _repoMock.Setup(r => r.ObtenerTodosPorRestauranteAsync(restauranteId))
                .ReturnsAsync(empleadosEsperados);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Juan", resultado[0].Nombre);
            _repoMock.Verify(r => r.ObtenerTodosPorRestauranteAsync(restauranteId), Times.Once);
        }
    }
}
