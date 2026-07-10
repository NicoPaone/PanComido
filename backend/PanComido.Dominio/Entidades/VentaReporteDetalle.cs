using System;

namespace PanComido.Dominio.Entidades
{
    public class VentaReporteDetalle
    {
        public int ComandaId { get; set; }
        public int NumeroMesa { get; set; }
        public DateTime FechaHora { get; set; }
        public int CantidadArticulos { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
    }
}
