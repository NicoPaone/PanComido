using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.EmpleadoCasosDeUso
{
    public class ModificarEmpleadoCasoDeUsoTest
    {
        private readonly Mock<IEmpleadoRepositorio> _repoMock;
        private readonly Mock<IContraseniaHasher> _hasherMock;
        private readonly ModificarEmpleadoCasoDeUso _casoDeUso;

        public ModificarEmpleadoCasoDeUsoTest()
        {
            _repoMock = new Mock<IEmpleadoRepositorio>();
            _hasherMock = new Mock<IContraseniaHasher>();
            _casoDeUso = new ModificarEmpleadoCasoDeUso(_repoMock.Object, _hasherMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoDatosValidos_ModificaEmpleado()
        {
            int restauranteId = 1;
            var empleado = new Empleado
            {
                Id = 1,
                Nombre = "Juan Modificado",
                Email = "juan@test.com",
                Rol = "Gerente",
                Estado = "activo"
            };
            var turnosIds = new List<int> { 2 };

            _repoMock.SetupSequence(r => r.ObtenerPorIdYRestauranteAsync(empleado.Id, restauranteId))
                .ReturnsAsync(new Empleado { Id = 1, Nombre = "Juan Viejo", Email = "juan@test.com", Rol = "Mozo" })
                .ReturnsAsync(empleado);

            _repoMock.Setup(r => r.ObtenerPorEmailAsync(empleado.Email))
                .ReturnsAsync((Empleado?)null);

            _repoMock.Setup(r => r.ModificarAsync(It.IsAny<Empleado>(), turnosIds))
                .Returns(Task.CompletedTask);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, empleado, null, turnosIds);

            Assert.NotNull(resultado);
            Assert.Equal("Juan Modificado", resultado.Nombre);
            Assert.Equal("Gerente", resultado.Rol);
            _repoMock.Verify(r => r.ModificarAsync(It.IsAny<Empleado>(), turnosIds), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEmpleadoNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            var empleado = new Empleado { Id = 99 };

            _repoMock.Setup(r => r.ObtenerPorIdYRestauranteAsync(empleado.Id, restauranteId))
                .ReturnsAsync((Empleado?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, empleado, null, new List<int>()));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEstadoEsInvalido_LanzaArgumentException()
        {
            var empleado = new Empleado
            {
                Id = 1,
                Nombre = "Juan",
                Email = "juan@test.com",
                Rol = "Mozo",
                Estado = "suspendido"
            };

            _repoMock.Setup(r => r.ObtenerPorIdYRestauranteAsync(empleado.Id, 1))
                .ReturnsAsync(new Empleado { Id = 1, Nombre = "Juan", Email = "juan@test.com", Rol = "Mozo" });

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, empleado, null, new List<int>()));
        }

        [Fact]
        public async Task EjecutarAsync_NormalizaRolYEstadoAntesDeGuardar()
        {
            var empleado = new Empleado
            {
                Id = 1,
                Nombre = "Juan",
                Email = "juan@test.com",
                Rol = "gerente",
                Estado = "INACTIVO"
            };

            Empleado? empleadoGuardado = null;

            _repoMock.SetupSequence(r => r.ObtenerPorIdYRestauranteAsync(empleado.Id, 1))
                .ReturnsAsync(new Empleado { Id = 1, Nombre = "Juan", Email = "juan@test.com", Rol = "Mozo" })
                .ReturnsAsync(() => empleadoGuardado);

            _repoMock.Setup(r => r.ObtenerPorEmailAsync(empleado.Email))
                .ReturnsAsync((Empleado?)null);

            _repoMock.Setup(r => r.ModificarAsync(It.IsAny<Empleado>(), It.IsAny<List<int>>()))
                .Callback<Empleado, List<int>>((e, _) => empleadoGuardado = e)
                .Returns(Task.CompletedTask);

            var resultado = await _casoDeUso.EjecutarAsync(1, empleado, null, new List<int>());

            Assert.NotNull(resultado);
            Assert.Equal(EmpleadoConstantes.RolGerente, resultado.Rol);
            Assert.Equal(EmpleadoConstantes.EstadoInactivo, resultado.Estado);
        }
    }
}
