namespace PanComido.Presentacion.DTOs.Restaurante
{
    public class RestauranteRequestDto
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public int? FamiliaTipograficaId { get; set; }
        public string? LinkResenaGoogleMaps { get; set; }
    }
}