using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using System.Collections.Generic;
using System.Linq;
namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class ReglaTiempoExtraEntityMapper
    {
        public DOM.ReglaTiempoExtra aDominio(EF.ReglaTiempoExtra entidad)
        {
            if (entidad == null) return null;
            return new DOM.ReglaTiempoExtra
            {
                Id = entidad.Id,
                RestauranteId = entidad.RestauranteId,
                PorcentajeOcupacionHasta = (int)entidad.PorcentajeOcupacionHasta,
                MinutosExtra = entidad.MinutosExtra
            };
        }
        public EF.ReglaTiempoExtra aEntidad(DOM.ReglaTiempoExtra dominio)
        {
            if (dominio == null) return null;
            return new EF.ReglaTiempoExtra
            {
                Id = dominio.Id,
                RestauranteId = dominio.RestauranteId,
                PorcentajeOcupacionHasta = dominio.PorcentajeOcupacionHasta,
                MinutosExtra = dominio.MinutosExtra
            };
        }

        public List<DOM.ReglaTiempoExtra> aDominios(List<EF.ReglaTiempoExtra> entidades)
        {
            return entidades?.Select(aDominio).ToList() ?? new List<DOM.ReglaTiempoExtra>();
        }
    }
}