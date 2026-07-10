using PanComido.Presentacion.DTOs.Insumos;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers;

public class CategoriaInsumoMapper
{
    public CategoriaInsumoResponseDto aDto(DOM.CategoriaInsumo categoria)
    {
        return new CategoriaInsumoResponseDto
        {
            Id = categoria.Id,
            Descripcion = categoria.Descripcion,
            TipoAplica = categoria.TipoAplica.ToString()
        };
    }

    public List<CategoriaInsumoResponseDto> aListaDto(
        List<DOM.CategoriaInsumo> categorias)
    {
        return categorias
            .Select(c => aDto(c))
            .ToList();
    }
}