using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades.IA;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers.IA
{
    public class SugerenciaIAEntityMapper
    {
        public DOM.SugerenciaIA aDominio(EF.SugerenciaPlatoIum entidad)
        {
            if (entidad == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<DOM.SugerenciaIA>(entidad.Json)
                    ?? throw new InvalidOperationException(
                        "No se pudo deserializar la sugerencia IA.");
        }

        public EF.SugerenciaPlatoIum aEntidad(int restauranteId,
                                                DOM.SugerenciaIA sugerencia)
        {
            if (sugerencia == null)
            {
                return null;
            }

            return new EF.SugerenciaPlatoIum
            {
                RestauranteId = restauranteId,
                Json = JsonSerializer.Serialize(sugerencia)
            };
        }

        public string aJson(DOM.SugerenciaIA sugerenciaIA)
        {
            return JsonSerializer.Serialize(sugerenciaIA);
        }
    }
}
