using System.Collections.Generic;

namespace PanComido.Presentacion.DTOs.Dashboard
{
    public class ResumenOperativoResponseDto
    {
        public string TotalVentas { get; set; } = string.Empty;
        public int TotalPedidos { get; set; }
        public string TicketPromedio { get; set; } = string.Empty;
        public int PromedioDiarioPedidos { get; set; }

        public string VariacionVentas { get; set; } = string.Empty;
        public string VariacionPedidos { get; set; } = string.Empty;
        public string VariacionTicket { get; set; } = string.Empty;

        public List<VentaAgrupadaDto> Grafico { get; set; } = new List<VentaAgrupadaDto>();
    }

    public class VentaAgrupadaDto
    {
        public string Etiqueta { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
    }
}
