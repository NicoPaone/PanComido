namespace PanComido.Presentacion.DTOs.Llamado
{
    public class LlamarMozoRequestDto
    {
        public int MesaId { get; set; }
        public int CategoriaLlamadoId { get; set; }
        public string Descripcion { get; set; }
    }
}
