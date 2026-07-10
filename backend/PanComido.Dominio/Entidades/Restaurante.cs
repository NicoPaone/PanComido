namespace PanComido.Dominio.Entidades
{
    public class Restaurante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string? ColorPrincipal { get; set; }
        public string? ColorSecundario { get; set; }
        public int DireccionId { get; set; }
        public Ubicacion Ubicacion { get; set; }
        public int? FamiliaTipograficaId { get; set; }
        public FamiliaTipografica? FamiliaTipografica { get; set; }
        public string? LinkResenaGoogleMaps { get; set; }
    }
}