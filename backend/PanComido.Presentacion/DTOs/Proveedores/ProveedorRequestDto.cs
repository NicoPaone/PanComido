using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Proveedores
{
    public class ProveedorRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 10 caracteres")]
        public string? NumeroTelefonoWsp { get; set; }

        [MinLength(1, ErrorMessage = "Debe seleccionar al menos una categoría.")]
        public List<int> CategoriaIds { get; set; } = new();
    }
}