namespace PanComido.Presentacion.DTOs
{
    public class BodegaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<InsumoResponseDto> Insumos { get; set; } = new List<InsumoResponseDto>();
    }
}
