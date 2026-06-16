using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.PorcetajesGanancia;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers

{
    public class PorcentajesGananciaMapper
    {
        public PorcentajeItemResponseDto aItemDto(DOM.PorcentajesCategoria itemGanancia)
        {
            return new PorcentajeItemResponseDto
            {
                Id = itemGanancia.Id,
                Descripcion = itemGanancia.Descripcion,
                Porcentaje = itemGanancia.Porcentaje
            };
        }

        public PorcentajesGananciaResponseDto aDto(DOM.PorcentajesGanancia porcentajesGanacia)
        {
            return new PorcentajesGananciaResponseDto
            {
                Platos = porcentajesGanacia.Platos.Select(aItemDto).ToList(),
                Bebidas = porcentajesGanacia.Bebidas.Select(aItemDto).ToList()
            };
        }

        public DOM.PorcentajesCategoria aItemDominio(PorcentajeItemRequestDto itemRequestDto)
        {
            return new PorcentajesCategoria
            {
                Id = itemRequestDto.Id,
                Porcentaje = itemRequestDto.Porcentaje
            };
        }

        public DOM.PorcentajesGanancia aDominio(PorcentajesGananciaRequestDto dto)
        {
            return new DOM.PorcentajesGanancia
            {
                Platos = dto.Platos.Select(aItemDominio).ToList(),
                Bebidas = dto.Bebidas.Select(aItemDominio).ToList()
            };
        }
    }
}
