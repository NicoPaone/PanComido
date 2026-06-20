namespace PanComido.Presentacion.DTOs.Articulo
{
    public class ArticuloComandaResponseDto
    {
        public int Id { get; set; }
        public bool Entregado { get; set; }
        public int Cantidad { get; set; }
        public string? ObservacionesGenerales { get; set; }
        public List<string> ObservacionesIngredientes { get; set; } = new List<string>();
        public ArticuloResponseDto Articulo { get; set; }
    }
}
