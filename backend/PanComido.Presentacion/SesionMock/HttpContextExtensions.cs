namespace PanComido.Presentacion.SesionMock
{
    public static class HttpContextExtensions
    {
        public static int ObtenerRestauranteId(this HttpContext context)
        {
            return (int)context.Items["restauranteId"]!;
        }
    }
}
