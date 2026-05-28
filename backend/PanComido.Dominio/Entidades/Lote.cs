namespace PanComido.Dominio.Entidades
{
    public class Lote
    {
        public int Id { get; set; }
        public int InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        // Relación de navegación
        public DateTime FechaVencimiento { get; set; }
    }
}