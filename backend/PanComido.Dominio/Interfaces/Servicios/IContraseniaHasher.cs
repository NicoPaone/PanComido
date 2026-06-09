namespace PanComido.Dominio.Interfaces.Servicios
{
   public interface IContraseniaHasher
   {
      string Hash(string contrasenia);
      bool Verificar(string input, string hash);

   }
}
