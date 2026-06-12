using PanComido.Presentacion.DTOs.FilaVirtual;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class FilaVirtualMapper
    {
        public FilaVirtualResponseDto aDto(DOM.FilaVirtual filaVirtual)
        {
            return new FilaVirtualResponseDto
            {
                Id = filaVirtual.Id,
                Habilitada = filaVirtual.Habilitada,
            };
        }

        public DOM.FilaVirtual aDominio(FilaVirtualRequestDto filaVirtualRequest)
        {
            return new DOM.FilaVirtual
            {
                Habilitada = filaVirtualRequest.Habilitada
            };
        }
    }
}
