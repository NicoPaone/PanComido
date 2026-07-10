using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class PorcentajesCategoriaEntityMapper
    {
        public DOM.PorcentajesCategoria paraDominio(EF.PorcentajeCategoriaPlato efPlato)
        {
            return new DOM.PorcentajesCategoria
            {
                Id = efPlato.CategoriaPlatoId,
                Descripcion = efPlato.CategoriaPlato.Descripcion,
                Porcentaje = efPlato.Porcentaje
            };
        }

        public DOM.PorcentajesCategoria paraDominio(EF.PorcentajeCategoriaBebidum efBebida)
        {
            return new DOM.PorcentajesCategoria
            {
                Id = efBebida.CategoriaInsumoId,
                Descripcion = efBebida.CategoriaInsumo.Descripcion,
                Porcentaje = efBebida.Porcentaje
            };
        }
    }
}
