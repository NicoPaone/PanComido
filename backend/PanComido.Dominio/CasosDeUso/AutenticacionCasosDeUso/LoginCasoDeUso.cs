
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso
{
   public class LoginCasoDeUso
   {
      private readonly IEmpleadoRepositorio _repositorio;
      private readonly IContraseniaHasher _hasher;

      public LoginCasoDeUso(IEmpleadoRepositorio repositorio, IContraseniaHasher hasher)
      {
         _repositorio = repositorio;
         _hasher = hasher;
      }
      public async Task<(Empleado Empleado, string Rol)> EjecutarAsync(string email, string contrasenia)
      {
         if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contrasenia))
            throw new ArgumentException("Email y contraseña son requeridos.");
         var empleado = await _repositorio.ObtenerPorEmailAsync(email);
         if (empleado == null)
            throw new UnauthorizedAccessException("Credenciales inválidas");

         if (empleado.Estado != "activo")
            throw new UnauthorizedAccessException("El empleado no está activo");

         if (!_hasher.Verificar(contrasenia, empleado.ContraseniaHash))
            throw new UnauthorizedAccessException("Credenciales inválidas");

         var rol = await _repositorio.ObtenerRolAsync(empleado.Id);
         if (rol == null)
            throw new UnauthorizedAccessException("El empleado no tiene un rol asignado");


         return (empleado,rol);



       }
   }
}
