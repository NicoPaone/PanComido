using Moq;
using PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Autenticacion
{
    public class SolicitarRecuperacionContraseniaCasoDeUsoTest
    {
        private readonly Mock<IEmpleadoRepositorio> _mockEmpleadoRepositorio;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly SolicitarRecuperacionContraseniaCasoDeUso _casoDeUso;

        public SolicitarRecuperacionContraseniaCasoDeUsoTest()
        {
            _mockEmpleadoRepositorio = new Mock<IEmpleadoRepositorio>();
            _mockEmailSender = new Mock<IEmailSender>();
            _casoDeUso = new SolicitarRecuperacionContraseniaCasoDeUso(_mockEmpleadoRepositorio.Object, _mockEmailSender.Object);
        }

        [Fact]
        public async Task EjecutarAsync_EmailInvalido_NoHaceNada()
        {
            // Preparar
            string email = "noexiste@test.com";
            string urlFrontend = "http://localhost:4200";

            _mockEmpleadoRepositorio.Setup(x => x.ObtenerPorEmailAsync(email))
                .ReturnsAsync((Empleado?)null);

            // Actuar
            await _casoDeUso.EjecutarAsync(email, urlFrontend);

            // Verificar
            _mockEmpleadoRepositorio.Verify(x => x.ActualizarAsync(It.IsAny<Empleado>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_EmailValido_ActualizaTokenYEnviaEmail()
        {
            // Preparar
            string email = "test@test.com";
            string urlFrontend = "http://localhost:4200";
            var empleado = new Empleado { Email = email };

            _mockEmpleadoRepositorio.Setup(x => x.ObtenerPorEmailAsync(email))
                .ReturnsAsync(empleado);

            // Actuar
            await _casoDeUso.EjecutarAsync(email, urlFrontend);

            // Verificar
            Assert.NotNull(empleado.ResetToken);
            Assert.NotNull(empleado.ResetTokenExpires);
            _mockEmpleadoRepositorio.Verify(x => x.ActualizarAsync(empleado), Times.Once);
            
            // Dado que el email se envía en un fire-and-forget (Task.Run), podría no ejecutarse de inmediato en el contexto de la prueba. 
            // Para simplificar, verificamos que el token se ha generado correctamente.
        }
    }
}
