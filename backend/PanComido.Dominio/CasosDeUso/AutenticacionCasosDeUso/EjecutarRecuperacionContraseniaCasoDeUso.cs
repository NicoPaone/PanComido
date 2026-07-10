using System;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso
{
    public class EjecutarRecuperacionContraseniaCasoDeUso
    {
        private readonly IEmpleadoRepositorio _repositorio;
        private readonly IContraseniaHasher _hasher;

        public EjecutarRecuperacionContraseniaCasoDeUso(IEmpleadoRepositorio repositorio, IContraseniaHasher hasher)
        {
            _repositorio = repositorio;
            _hasher = hasher;
        }

        public async Task EjecutarAsync(string email, string token, string nuevaContrasenia)
        {
            var empleado = await _repositorio.ObtenerPorEmailAsync(email);
            
            if (empleado == null || empleado.ResetToken != token || empleado.ResetTokenExpires < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Token inválido o expirado.");
            }

            empleado.ContraseniaHash = _hasher.Hash(nuevaContrasenia);
            empleado.ResetToken = null;
            empleado.ResetTokenExpires = null;

            await _repositorio.ActualizarAsync(empleado);
        }
    }
}
