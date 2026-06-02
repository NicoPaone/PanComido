namespace PanComido.Presentacion.DTOs.Articulos
{
    public class ArticuloCartaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string UrlImagen { get; set; }

        public bool EsPlato { get; set; }
        public bool EsDestacado { get; set; }
        public int? TiempoPreparacionBase { get; set; }

        public string CategoriaPlato { get; set; }
        public string CategoriaBebida { get; set; }
        public List<string> Restricciones { get; set; } = new List<string>();
    }
}
