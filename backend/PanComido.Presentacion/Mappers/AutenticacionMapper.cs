using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Autenticacion;

namespace PanComido.Presentacion.Mappers
{
   public class AutenticacionMapper
   {
      public LoginResponseDto  aResponseDto(Empleado empleado, string token, string rol)
      {
         return new LoginResponseDto
         {
            Token = token,
            Rol = rol,
            Nombre = empleado.Nombre
         };
      }
   }
}
