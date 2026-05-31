namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class PreRecepcionItemResponseDto
    {
        public int InsumoId { get; set; }
        public string NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public string NombreLote { get; set; }
        public int BodegaIdSug { get; set; }
        public string FechaVencimientoSug { get; set; }
    }
}
