using Moq;
using PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Autenticacion
{
    public class EjecutarRecuperacionContraseniaCasoDeUsoTest
    {
        private readonly Mock<IEmpleadoRepositorio> _mockEmpleadoRepositorio;
        private readonly Mock<IContraseniaHasher> _mockHasher;
        private readonly EjecutarRecuperacionContraseniaCasoDeUso _casoDeUso;

        public EjecutarRecuperacionContraseniaCasoDeUsoTest()
        {
            _mockEmpleadoRepositorio = new Mock<IEmpleadoRepositorio>();
            _mockHasher = new Mock<IContraseniaHasher>();
            _casoDeUso = new EjecutarRecuperacionContraseniaCasoDeUso(_mockEmpleadoRepositorio.Object, _mockHasher.Object);
        }

        [Fact]
        public async Task EjecutarAsync_TokenValido_ActualizaContrasenia()
        {
            // Preparar
            string email = "test@test.com";
            string token = "token123";
            string nuevaContrasenia = "nueva123";
            var empleado = new Empleado 
            { 
                Email = email, 
                ResetToken = token, 
                ResetTokenExpires = DateTime.UtcNow.AddMinutes(5) 
            };

            _mockEmpleadoRepositorio.Setup(x => x.ObtenerPorEmailAsync(email))
                .ReturnsAsync(empleado);

            _mockHasher.Setup(x => x.Hash(nuevaContrasenia))
                .Returns("hashedPassword");

            // Actuar
            await _casoDeUso.EjecutarAsync(email, token, nuevaContrasenia);

            // Verificar
            Assert.Equal("hashedPassword", empleado.ContraseniaHash);
            Assert.Null(empleado.ResetToken);
            Assert.Null(empleado.ResetTokenExpires);
            _mockEmpleadoRepositorio.Verify(x => x.ActualizarAsync(empleado), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_TokenInvalido_LanzaExcepcion()
        {
            // Preparar
            string email = "test@test.com";
            string token = "token123";
            string nuevaContrasenia = "nueva123";
            var empleado = new Empleado 
            { 
                Email = email, 
                ResetToken = "tokenDistinto", 
                ResetTokenExpires = DateTime.UtcNow.AddMinutes(5) 
            };

            _mockEmpleadoRepositorio.Setup(x => x.ObtenerPorEmailAsync(email))
                .ReturnsAsync(empleado);

            // Actuar & Verificar
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _casoDeUso.EjecutarAsync(email, token, nuevaContrasenia));
        }
    }
}
