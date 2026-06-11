using Moq;
using PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Autenticacion
{
    public class LoginCasoDeUsoTest
    {
        private readonly Mock<IEmpleadoRepositorio> _empleadoMockRepo;
        private readonly Mock<IContraseniaHasher> _hasherMock;

        public LoginCasoDeUsoTest()
        {
            _empleadoMockRepo = new Mock<IEmpleadoRepositorio>();
            _hasherMock = new Mock<IContraseniaHasher>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoCredencialesSonCorrectas_RetornaEmpleadoYRol()
        {
            var email = "gerente@pancomido.com";
            var contrasenia = "12341234";
            var empleado = new DOM.Empleado
            {
                Id = 1,
                RestauranteId = 1,
                Nombre = "Gerente Test",
                Email = email,
                ContraseniaHash = "$2a$11$hash",
                Estado = "activo"
            };

            _empleadoMockRepo
                .Setup(r => r.ObtenerPorEmailAsync(email))
                .ReturnsAsync(empleado);
            _hasherMock
                .Setup(h => h.Verificar(contrasenia, empleado.ContraseniaHash))
                .Returns(true);
            _empleadoMockRepo
                .Setup(r => r.ObtenerRolAsync(empleado.Id))
                .ReturnsAsync("Gerente");

            var casoDeUso = new LoginCasoDeUso(_empleadoMockRepo.Object, _hasherMock.Object);

            var (empleadoResultado, rol) = await casoDeUso.EjecutarAsync(email, contrasenia);

            Assert.NotNull(empleadoResultado);
            Assert.Equal("Gerente", rol);
            Assert.Equal(email, empleadoResultado.Email);
        }

        [Theory]
        [InlineData("", "1234")]
        [InlineData("email@test.com", "")]
        [InlineData("", "")]
        public async Task EjecutarAsync_CuandoEmailOContraseniaEstanVacios_LanzaArgumentException(string email, string contrasenia)
        {
            var casoDeUso = new LoginCasoDeUso(_empleadoMockRepo.Object, _hasherMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(email, contrasenia));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEmpleadoNoExiste_LanzaUnauthorizedAccessException()
        {
            _empleadoMockRepo
                .Setup(r => r.ObtenerPorEmailAsync("inexistente@test.com"))
                .ReturnsAsync((DOM.Empleado?)null);

            var casoDeUso = new LoginCasoDeUso(_empleadoMockRepo.Object, _hasherMock.Object);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                casoDeUso.EjecutarAsync("inexistente@test.com", "1234"));

            Assert.Equal("Credenciales inválidas", ex.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEmpleadoEstaInactivo_LanzaUnauthorizedAccessException()
        {
            var email = "inactivo@test.com";
            var empleado = new DOM.Empleado
            {
                Id = 2,
                Email = email,
                ContraseniaHash = "$2a$11$hash",
                Estado = "inactivo"
            };

            _empleadoMockRepo
                .Setup(r => r.ObtenerPorEmailAsync(email))
                .ReturnsAsync(empleado);

            var casoDeUso = new LoginCasoDeUso(_empleadoMockRepo.Object, _hasherMock.Object);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                casoDeUso.EjecutarAsync(email, "1234"));

            Assert.Equal("El empleado no está activo", ex.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoContraseniaEsIncorrecta_LanzaUnauthorizedAccessException()
        {
            var email = "test@test.com";
            var empleado = new DOM.Empleado
            {
                Id = 3,
                Email = email,
                ContraseniaHash = "$2a$11$hash",
                Estado = "activo"
            };

            _empleadoMockRepo
                .Setup(r => r.ObtenerPorEmailAsync(email))
                .ReturnsAsync(empleado);
            _hasherMock
                .Setup(h => h.Verificar("wrong", empleado.ContraseniaHash))
                .Returns(false);

            var casoDeUso = new LoginCasoDeUso(_empleadoMockRepo.Object, _hasherMock.Object);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                casoDeUso.EjecutarAsync(email, "wrong"));

            Assert.Equal("Credenciales inválidas", ex.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEmpleadoNoTieneRolAsignado_LanzaUnauthorizedAccessException()
        {
            var email = "sinrol@test.com";
            var empleado = new DOM.Empleado
            {
                Id = 4,
                Email = email,
                ContraseniaHash = "$2a$11$hash",
                Estado = "activo"
            };

            _empleadoMockRepo
                .Setup(r => r.ObtenerPorEmailAsync(email))
                .ReturnsAsync(empleado);
            _hasherMock
                .Setup(h => h.Verificar("1234", empleado.ContraseniaHash))
                .Returns(true);
            _empleadoMockRepo
                .Setup(r => r.ObtenerRolAsync(empleado.Id))
                .ReturnsAsync((string?)null);

            var casoDeUso = new LoginCasoDeUso(_empleadoMockRepo.Object, _hasherMock.Object);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                casoDeUso.EjecutarAsync(email, "1234"));

            Assert.Equal("El empleado no tiene un rol asignado", ex.Message);
        }
    }
}
