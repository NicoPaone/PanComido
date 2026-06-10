
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Infraestructura.ServiciosExternos
{
   public class ContraseniaHasher : IContraseniaHasher
   {

      public string Hash(string contrasenia)
      {
         return BCrypt.Net.BCrypt.HashPassword(contrasenia);
      }

      public bool Verificar(string input, string hash)
      {
         return BCrypt.Net.BCrypt.Verify(input, hash);
      }
   }
}
