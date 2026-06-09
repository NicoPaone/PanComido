namespace PanComido.Presentacion.DTOs.Autenticacion
{
   public class LoginResponseDto
   {
      public string Token { get; set; } = string.Empty;
      public string Rol { get; set; } = string.Empty;
      public string Nombre {  get; set; } = string.Empty;
   }
}
