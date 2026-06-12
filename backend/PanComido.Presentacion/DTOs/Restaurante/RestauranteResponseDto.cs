namespace PanComido.Presentacion.DTOs.Restaurante
{
    public class RestauranteResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string Direccion { get; set; }
        public int? FamiliaTipograficaId { get; set; }
        public string? FamiliaCategoria { get; set; }
        public string? TipografiaTitulo { get; set; }
        public string? TipografiaCuerpo { get; set; }
    }
}