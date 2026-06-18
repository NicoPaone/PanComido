using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Mesas
{
    public class GuardarMesaRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de mesa es requerido.")]
        [Range(0, 999, ErrorMessage = "El número de mesa debe ser 0 o mayor.")]
        public int NumeroMesa { get; set; }

        [Required(ErrorMessage = "La capacidad es requerida.")]
        [Range(0, 100, ErrorMessage = "La capacidad debe ser 0 o mayor.")]
        public int CantidadPersonasMax { get; set; }

        [Required]
        public string EstadoMesa { get; set; }

        [Required]
        [Range(0, 3000, ErrorMessage = "La posición X debe estar entre 0 y 3000 píxeles.")]
        public int PosicionXInicio { get; set; }

        [Required]
        [Range(0, 3000, ErrorMessage = "La posición X debe estar entre 0 y 3000 píxeles.")]
        public int PosicionXFin { get; set; }

        [Required]
        [Range(0, 3000, ErrorMessage = "La posición Y debe estar entre 0 y 3000 píxeles.")]
        public int PosicionYInicio { get; set; }

        [Required]
        [Range(0, 3000, ErrorMessage = "La posición Y debe estar entre 0 y 3000 píxeles.")]
        public int PosicionYFin { get; set; }

        [Required]
        public DimensionMesaDto DimensionMesa { get; set; }

        public int TipoElemento { get; set; } = 1;
        public string? Color { get; set; }
        public string? TextoObjeto { get; set; }
    }

    
}
