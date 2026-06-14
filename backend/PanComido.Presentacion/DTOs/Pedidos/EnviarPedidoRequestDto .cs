using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class ConfirmarPedidoRequestDto
    {
        [MinLength(1, ErrorMessage = "Debe incluir al menos un insumo")]
        public List<CrearPedidoItemDto> ListaInsumosPedido { get; set; }
    }
}