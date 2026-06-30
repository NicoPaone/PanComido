using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Entidades
{
    public class Pago
    {
        public int PagoId { get; set; }
        public int ComandaId { get; set; }
        public int? CierreId { get; set; }
        public MetodoPago MetodoDePago { get; set; }
        public string? ExternalReference { get; set; }
        public decimal Total { get; set; }
        public EstadoPago EstadoPago { get; set; }
        //public DateTime FechaHoraPago { get; set; }
    }
}