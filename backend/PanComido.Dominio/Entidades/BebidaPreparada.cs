namespace PanComido.Dominio.Entidades
{
    public class BebidaPreparada : Articulo
    {
        public List<BebidaPreparadaInsumo> Insumos { get; set; } = new List<BebidaPreparadaInsumo>();

        public string Categoria { get; set; }
    }
}
