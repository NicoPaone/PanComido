using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.EmpleadoCasosDeUso
{
    public class EliminarEmpleadoCasoDeUsoTest
    {
        private readonly Mock<IEmpleadoRepositorio> _repoMock;
        private readonly EliminarEmpleadoCasoDeUso _casoDeUso;

        public EliminarEmpleadoCasoDeUsoTest()
        {
            _repoMock = new Mock<IEmpleadoRepositorio>();
            _casoDeUso = new EliminarEmpleadoCasoDeUso(_repoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExiste_EliminaLogicamente()
        {
            int id = 1;
            int restauranteId = 1;

            _repoMock.Setup(r => r.ObtenerPorIdYRestauranteAsync(id, restauranteId))
                .ReturnsAsync(new Empleado { Id = id, Nombre = "Juan" });

            _repoMock.Setup(r => r.EliminarLogicoAsync(id, restauranteId))
                .Returns(Task.CompletedTask);

            await _casoDeUso.EjecutarAsync(id, restauranteId);

            _repoMock.Verify(r => r.EliminarLogicoAsync(id, restauranteId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoExiste_LanzaKeyNotFoundException()
        {
            int id = 99;
            int restauranteId = 1;

            _repoMock.Setup(r => r.ObtenerPorIdYRestauranteAsync(id, restauranteId))
                .ReturnsAsync((Empleado?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _casoDeUso.EjecutarAsync(id, restauranteId));
        }
    }
}
