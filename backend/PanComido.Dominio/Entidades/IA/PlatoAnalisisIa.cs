using System.Collections.Generic;

namespace PanComido.Dominio.Entidades.IA
{
    public class PlatoAnalisisIa
    {
        public int PlatoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public string Alerta { get; set; } = "moderada"; // "critica" | "moderada"
        public List<PlatoSugerenciaIa> Sugerencias { get; set; } = new List<PlatoSugerenciaIa>();
    }
}
