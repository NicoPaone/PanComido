namespace PanComido.Presentacion.DTOs.Restaurante
{
    public class RestauranteRequestDto
    {
        public string Nombre { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public int? FamiliaTipograficaId { get; set; }
    }
}