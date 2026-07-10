using Microsoft.AspNetCore.Http;
using PanComido.Presentacion.DTOs.ErrorResponse;

namespace PanComido.Presentacion.Controllers
{
    internal static class ApiErrorResponseFactory
    {
        public static ErrorResponseDto Crear(HttpContext httpContext, string mensaje, string codigo)
        {
            return new ErrorResponseDto
            {
                Error = mensaje,
                Code = codigo,
                TraceId = httpContext.TraceIdentifier
            };
        }
    }
}
