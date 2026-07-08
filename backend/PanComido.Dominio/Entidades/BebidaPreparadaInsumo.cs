namespace PanComido.Dominio.Entidades
{
    public class BebidaPreparadaInsumo
    {
        public int InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public Insumo Insumo { get; set; }
    }
}
