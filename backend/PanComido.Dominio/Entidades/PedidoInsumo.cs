namespace PanComido.Dominio.Entidades
{
    public class PedidoInsumo
    {
        public int InsumoId { get; set; }
        public string NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public string UnidadMedida { get; set; }
        public int CategoriaInsumoId { get; set; }
    }
}