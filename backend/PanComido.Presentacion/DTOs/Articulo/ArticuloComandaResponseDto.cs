namespace PanComido.Presentacion.DTOs.Articulo
{
    public class ArticuloComandaResponseDto
    {
        public int Id { get; set; }
        public bool Entregado { get; set; }
        public int Cantidad { get; set; }
        public string? ObservacionesGenerales { get; set; }
        public string? ObservacionesIngredientes { get; set; }

        public ArticuloResponseDto Articulo { get; set; }
    }
}
