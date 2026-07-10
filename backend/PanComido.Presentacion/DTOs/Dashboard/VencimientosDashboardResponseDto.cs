using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.DTOs.Dashboard
{
    public class VencimientosDashboardResponseDto
    {
        public List<InsumoPorVencerDto> InsumosPorVencer { get; set; } = new();
    }

    public class InsumoPorVencerDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string LoteNombre { get; set; } = string.Empty; // El item chiquito nuevo
        public string Fecha { get; set; } = string.Empty;       // Ej: "09/06" o "10/06" (como la foto)
        public string Cantidad { get; set; } = string.Empty;    // Ej: "3 Lt disponibles", "4.75 Kg disponibles"
        public string Criticidad { get; set; } = string.Empty;  // "ALTA", "MEDIA", "BAJA" (para el texto de la derecha)
        public string Relativo { get; set; } = string.Empty;    // Ej: "vence mañana", "vence en 2 días"
    }
}
