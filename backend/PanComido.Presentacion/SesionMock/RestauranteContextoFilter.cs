using Microsoft.AspNetCore.Mvc.Filters;

namespace PanComido.Presentacion.SesionMock
{
    public class RestauranteContextoFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // obtener id
            // Por ahora hardcodeado, luego de dinamico
            context.HttpContext.Items["restauranteId"] = 1;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
