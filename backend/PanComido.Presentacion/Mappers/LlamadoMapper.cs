using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.DTOs.Llamado;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class LlamadoMapper
    {
        public LlamadoResponseDto aDto(DOM.Llamado llamado)
        {
            return new LlamadoResponseDto
            {
                Id = llamado.Id,
                MozoId = llamado.MozoId,
                CategoriaLlamadoId = llamado.CategoriaLlamadoId,
                CategoriaDescripcion = llamado.CategoriaDescripcion,
                Descripcion = llamado.Descripcion,
                Resuelto = llamado.Resuelto
            };
        }

        public List<LlamadoResponseDto> aListaDto(List<DOM.Llamado> llamados)
        {
            return llamados
                .Select(i => aDto(i))
                .ToList();
        }
    }
}
