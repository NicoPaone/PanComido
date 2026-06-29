using System.Collections.Generic;

namespace PanComido.Presentacion.DTOs.Dashboard
{
    public class AplicarDescuentoRequest
    {
        public int PlatoId { get; set; }
        public decimal PorcentajeDescuento { get; set; }
    }

    public class AplicarDescuentoResponse
    {
        public string Mensaje { get; set; } = string.Empty;
        public int PlatoId { get; set; }
        public decimal PrecioNuevo { get; set; }
        public decimal Costo { get; set; }
        public string MargenPctNuevo { get; set; } = string.Empty;
    }

    public class AgendarRecordatorioRequest
    {
        public int PlatoId { get; set; }
        public string AccionSugerida { get; set; } = string.Empty;
    }

    public class AgendarRecordatorioResponse
    {
        public string Mensaje { get; set; } = string.Empty;
        public DashboardAccionItemDto AccionItem { get; set; } = new DashboardAccionItemDto();
    }

    public class DashboardAccionItemDto
    {
        public int? Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public string Destino { get; set; } = "carta";
        public string Tono { get; set; } = "info";
        public string Impacto { get; set; } = "Reevaluar demanda";
        public int Prioridad { get; set; } = 4;
    }

    public class PlatoAnalisisDto
    {
        public int PlatoId { get; set; }
        public DashboardRankingItemDto Plato { get; set; } = new DashboardRankingItemDto();
        public string Diagnostico { get; set; } = string.Empty;
        public string Alerta { get; set; } = "moderada";
        public MetricasAnalisisDto Metricas { get; set; } = new MetricasAnalisisDto();
        public ComparativaAnalisisDto Comparativa { get; set; } = new ComparativaAnalisisDto();
        public List<int> Tendencia { get; set; } = new List<int>();
        public List<PlatoSugerenciaDto> SugerenciasDetalladas { get; set; } = new List<PlatoSugerenciaDto>();
    }

    public class DashboardRankingItemDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Valor { get; set; }
        public string Detalle { get; set; } = string.Empty;
    }

    public class MetricasAnalisisDto
    {
        public string Volumen { get; set; } = string.Empty;
        public string VolumenVar { get; set; } = string.Empty;
        public string Costo { get; set; } = string.Empty;
        public string Precio { get; set; } = string.Empty;
        public string MargenPct { get; set; } = string.Empty;
        public string Participacion { get; set; } = string.Empty;
    }

    public class ComparativaAnalisisDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Precio { get; set; } = string.Empty;
        public string Ventas { get; set; } = string.Empty;
    }

    public class PlatoSugerenciaDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Impacto { get; set; } = string.Empty;
        public string Dificultad { get; set; } = string.Empty;
        public bool EsAplicable { get; set; }
        public bool Aplicada { get; set; }
    }
}
