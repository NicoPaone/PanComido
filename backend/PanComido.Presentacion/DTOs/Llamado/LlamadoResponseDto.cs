namespace PanComido.Presentacion.DTOs.Llamado
{
    public class LlamadoResponseDto
    {
        public int Id { get; set; }
        public int? MozoId { get; set; }
        public int? MesaId { get; set; }
        public int CategoriaLlamadoId { get; set; }
        public string CategoriaDescripcion { get; set; }
        public string Descripcion { get; set; }
        public bool Resuelto { get; set; }
    }
}
