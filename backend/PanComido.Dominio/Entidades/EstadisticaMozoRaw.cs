namespace PanComido.Dominio.Entidades
{
    public class EstadisticaMozoRaw
    {
        public string Nombre { get; set; } = string.Empty;
        public int MesasAtendidas { get; set; }
        public decimal FacturacionTotal { get; set; }
        public double? MinutosPromedioAtencion { get; set; }
        public int ComandasActivas { get; set; }
        public double? PromedioEstrellas { get; set; }

    }
}
