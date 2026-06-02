namespace PanComido.Presentacion.SesionMock
{
    public static class HttpContextExtensions
    {
        public static int ObtenerRestauranteId(this HttpContext context)
        {
            return (int)context.Items["restauranteId"]!;
        }

        public static int ObtenerMozoId(this HttpContext context)
        {
            return (int)context.Items["mozoId"]!;
        }
    }
}
