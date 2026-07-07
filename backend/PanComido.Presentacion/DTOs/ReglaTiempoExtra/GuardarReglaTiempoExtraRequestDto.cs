using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.ReglaTiempoExtra
{
    public class GuardarReglaTiempoExtraRequestDto
    {
        [Range(1, 100, ErrorMessage = "El porcentaje debe estar entre 1 y 100.")]
        public int PorcentajeOcupacionHasta { get; set; }

        [Range(0, 1000, ErrorMessage = "Los minutos extra deben ser positivos.")]
        public int MinutosExtra { get; set; }
    }
}
