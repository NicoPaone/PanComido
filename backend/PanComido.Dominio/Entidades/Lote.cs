namespace PanComido.Dominio.Entidades
{
    public class Lote
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public DateOnly FechaAdquisicion { get; set; }
        public DateOnly FechaVencimiento { get; set; }

        public int BodegaId { get; set; }
    }
}