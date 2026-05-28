namespace PanComido.Presentacion.DTOs
{
    public class InsumoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal StockActual { get; set; }   // calculado desde lotes
        public string UnidadMedida { get; set; }   // "KG", "L", etc.
        public string? Vencimiento { get; set; }   // "dd/MM/yyyy" o null
        public decimal StockMinimo { get; set; }
        public string EstadoStock { get; set; }   // "Critico" | "Bajo" | "Normal"
        public string Tipo { get; set; }   // "Ingrediente" | "Bebida"
        public string? Categoria { get; set; }
    }
}
