using System.Collections.Generic;
using PanComido.Presentacion.DTOs.TurnoLaboral;

namespace PanComido.Presentacion.DTOs.Empleado
{
    public class EmpleadoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public List<TurnoLaboralResponseDto> Turnos { get; set; } = new();
    }
}
