using Microsoft.AspNetCore.Diagnostics;

namespace PanComido.Presentacion
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            switch (exception)
            {
                case ArgumentException:
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsJsonAsync(new { error = exception.Message });
                    break;
                case KeyNotFoundException:
                    httpContext.Response.StatusCode = 404;
                    await httpContext.Response.WriteAsJsonAsync(new { error = exception.Message });
                    break;
                case UnauthorizedAccessException:
                    httpContext.Response.StatusCode = 401;
                    await httpContext.Response.WriteAsJsonAsync(new { error = exception.Message });
                    break;
                case InvalidOperationException:
                    httpContext.Response.StatusCode = 409;
                    await httpContext.Response.WriteAsJsonAsync(new { error = exception.Message });
                    break;
                default:
                    httpContext.Response.StatusCode = 500;
                    await httpContext.Response.WriteAsJsonAsync(new { error = "Error interno del servidor." });
                    break;
            }
            return await ValueTask.FromResult(true);
        }
    }
}
