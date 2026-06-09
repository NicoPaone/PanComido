namespace PanComido.Presentacion.Sesion
{
    public static class HttpContextExtensions
    {
      public static int ObtenerRestauranteId(this HttpContext context)
      {
         if (context.Items.TryGetValue("restauranteId", out var id))
            return (int)id;

         throw new UnauthorizedAccessException("El token no contiene un RestauranteId válido.");
      }

      public static string ObtenerRol(this HttpContext context)
      {
         if (context.Items.TryGetValue("rol", out var rol))
            return (string)rol;

         throw new UnauthorizedAccessException("El token no contiene un Rol válido.");
      }

      public static int ObtenerEmpleadoId(this HttpContext context)
      {
         if (context.Items.TryGetValue("empleadoId", out var id))
            return (int)id;

         throw new UnauthorizedAccessException("El token no contiene un EmpleadoId válido.");
      }
   }
}
