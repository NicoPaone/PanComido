using System;
using System.Collections.Generic;

namespace PanComido.Dominio.Entidades
{
    public class ResumenOperativo
    {
        public decimal TotalVentas { get; set; }
        public int TotalPedidos { get; set; }
        public decimal TicketPromedio { get; set; }
        public int PromedioDiarioPedidos { get; set; }

        public decimal VariacionVentas { get; set; }
        public decimal VariacionPedidos { get; set; }
        public decimal VariacionTicket { get; set; }

        public List<VentaAgrupada> Grafico { get; set; } = new List<VentaAgrupada>();
        public List<DashboardAccionItem> Recordatorios { get; set; } = new List<DashboardAccionItem>();
    }

    public class DashboardAccionItem
    {
        public int? Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public string Destino { get; set; } = "carta";
        public string Tono { get; set; } = "info";
        public string Impacto { get; set; } = "Reevaluar demanda";
        public int Prioridad { get; set; } = 4;
    }

    public class TotalesPeriodo
    {
        public decimal TotalFacturado { get; set; }
        public int CantidadPedidos { get; set; }
    }

    public class VentaAgrupada
    {
        public string Etiqueta { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
