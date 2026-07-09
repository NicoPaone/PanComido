using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.ValueObjects;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.EmpleadoCasosDeUso
{
    public class CrearEmpleadoCasoDeUsoTest
    {
        private readonly Mock<IEmpleadoRepositorio> _repoMock;
        private readonly Mock<IContraseniaHasher> _hasherMock;
        private readonly CrearEmpleadoCasoDeUso _casoDeUso;

        public CrearEmpleadoCasoDeUsoTest()
        {
            _repoMock = new Mock<IEmpleadoRepositorio>();
            _hasherMock = new Mock<IContraseniaHasher>();
            _casoDeUso = new CrearEmpleadoCasoDeUso(_repoMock.Object, _hasherMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoDatosValidos_CreaEmpleado()
        {
            int restauranteId = 1;
            var empleado = new Empleado
            {
                Nombre = "Juan",
                Email = "juan@test.com",
                Rol = "Mozo"
            };
            string contrasenia = "pass123";
            var turnosIds = new List<int> { 1 };

            _repoMock.Setup(r => r.ObtenerPorEmailAsync(empleado.Email))
                .ReturnsAsync((Empleado?)null);

            _hasherMock.Setup(h => h.Hash(contrasenia))
                .Returns("hashedPassword");

            _repoMock.Setup(r => r.CrearAsync(It.IsAny<Empleado>(), turnosIds))
                .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.ObtenerPorIdYRestauranteAsync(It.IsAny<int>(), restauranteId))
                .ReturnsAsync(new Empleado { Id = 1, Nombre = "Juan", Email = "juan@test.com", Rol = "Mozo" });

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, empleado, contrasenia, turnosIds);

            Assert.NotNull(resultado);
            Assert.Equal("Juan", resultado.Nombre);
            Assert.Equal("Mozo", resultado.Rol);
            _repoMock.Verify(r => r.CrearAsync(It.IsAny<Empleado>(), turnosIds), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEmailDuplicado_LanzaArgumentException()
        {
            int restauranteId = 1;
            var empleado = new Empleado
            {
                Nombre = "Juan",
                Email = "juan@test.com",
                Rol = "Mozo"
            };

            _repoMock.Setup(r => r.ObtenerPorEmailAsync(empleado.Email))
                .ReturnsAsync(new Empleado { Id = 2, Email = "juan@test.com" });

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, empleado, "pass123", new List<int>()));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoRolEsInvalido_LanzaArgumentException()
        {
            var empleado = new Empleado
            {
                Nombre = "Juan",
                Email = "juan@test.com",
                Rol = "Admin",
                Estado = "activo"
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, empleado, "pass123", new List<int>()));
        }

        [Fact]
        public async Task EjecutarAsync_NormalizaRolYEstado()
        {
            var empleado = new Empleado
            {
                Nombre = "Juan",
                Email = "juan@test.com",
                Rol = "mozo",
                Estado = "ACTIVO"
            };

            _repoMock.Setup(r => r.ObtenerPorEmailAsync(empleado.Email))
                .ReturnsAsync((Empleado?)null);

            _hasherMock.Setup(h => h.Hash("pass123"))
                .Returns("hashedPassword");

            _repoMock.Setup(r => r.CrearAsync(It.IsAny<Empleado>(), It.IsAny<List<int>>()))
                .Returns(Task.CompletedTask)
                .Callback<Empleado, List<int>>((e, _) => e.Id = 10);

            _repoMock.Setup(r => r.ObtenerPorIdYRestauranteAsync(10, 1))
                .ReturnsAsync((Empleado?)null);

            var resultado = await _casoDeUso.EjecutarAsync(1, empleado, "pass123", new List<int>());

            Assert.Equal(RolEmpleado.Mozo, resultado.Rol);
            Assert.Equal(EstadoEmpleado.Activo, resultado.Estado);
        }
    }
}
