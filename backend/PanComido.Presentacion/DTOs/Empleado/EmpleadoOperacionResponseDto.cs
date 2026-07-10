namespace PanComido.Presentacion.DTOs.Empleado
{
    public class EmpleadoOperacionResponseDto
    {
        public string Mensaje { get; set; } = string.Empty;
        public EmpleadoResponseDto Empleado { get; set; } = new EmpleadoResponseDto();
    }
}
