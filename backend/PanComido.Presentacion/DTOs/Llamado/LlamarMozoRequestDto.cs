using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Llamado
{
    public class LlamarMozoRequestDto
    {
        public int MesaId { get; set; }
        public int CategoriaLlamadoId { get; set; }

        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
        public string Descripcion { get; set; }
        public int restauranteId { get; set; }
    }
}