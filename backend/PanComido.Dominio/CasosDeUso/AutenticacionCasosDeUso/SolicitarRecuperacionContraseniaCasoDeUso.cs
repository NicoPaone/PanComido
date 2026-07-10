using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso
{
    public class SolicitarRecuperacionContraseniaCasoDeUso
    {
        private readonly IEmpleadoRepositorio _repositorio;
        private readonly IEmailSender _emailSender;

        public SolicitarRecuperacionContraseniaCasoDeUso(IEmpleadoRepositorio repositorio, IEmailSender emailSender)
        {
            _repositorio = repositorio;
            _emailSender = emailSender;
        }

        public async Task EjecutarAsync(string email, string urlFrontend)
        {
            var empleado = await _repositorio.ObtenerPorEmailAsync(email);
            if (empleado == null) return;

            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(tokenBytes);
            
            empleado.ResetToken = token;
            empleado.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15);
            
            await _repositorio.ActualizarAsync(empleado);

            var link = $"{urlFrontend}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
            var cuerpo = $"<p>Ingresá al siguiente enlace para recuperar tu clave:</p><p><a href='{link}'>{link}</a></p><p>Este enlace expira en 15 minutos.</p>";
            
            // Usamos fire-and-forget para que el frontend no se quede cargando 
            // indefinidamente si el servidor SMTP de Google tarda en responder.
            _ = Task.Run(() => _emailSender.EnviarEmailAsync(email, "Recuperación de Contraseña", cuerpo));
        }
    }
}
