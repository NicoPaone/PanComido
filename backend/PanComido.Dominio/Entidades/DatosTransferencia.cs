namespace PanComido.Dominio.Entidades
{
    public class DatosTransferencia
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public string Alias { get; set; }
        public string? Cbu { get; set; }
        public string NumeroCuenta { get; set; }
        public string TitularCuenta { get; set; }
    }
}
