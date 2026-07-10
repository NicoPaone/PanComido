using System.Collections.Generic;

namespace PanComido.Dominio.Entidades.IA
{
    public class PlatoAnalisisIa
    {
        public int PlatoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public string Alerta { get; set; } = "moderada"; // "critica" | "moderada"
        public string FuenteAnalisis { get; set; } = "desconocida";
        public bool EsFallbackLocal { get; set; }
        public string? MotivoFallback { get; set; }
        public List<PlatoSugerenciaIa> Sugerencias { get; set; } = new List<PlatoSugerenciaIa>();
    }
}
