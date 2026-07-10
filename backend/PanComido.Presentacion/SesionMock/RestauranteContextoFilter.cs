using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PanComido.Presentacion.Filtros
{
   public class RestauranteContextoFilter : IActionFilter
   {
      public void OnActionExecuting(ActionExecutingContext context)
      {
         //usuario decodificado por el middleware de JWT
         var usuario = context.HttpContext.User;

         if (usuario.Identity?.IsAuthenticated != true)
            return;


         var restauranteId = usuario.FindFirst("restauranteId")?.Value;
         var rol = usuario.FindFirst(ClaimTypes.Role)?.Value;
         var empleadoId= usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

         if (!string.IsNullOrEmpty(restauranteId))
            context.HttpContext.Items["restauranteId"] = int.Parse(restauranteId);

         if (!string.IsNullOrEmpty(rol))
            context.HttpContext.Items["rol"] = rol;

         if (!string.IsNullOrEmpty(empleadoId))
            context.HttpContext.Items["empleadoId"] = int.Parse(empleadoId);

      }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
