using PanComido.Dominio.Entidades.IA;
using PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.Mappers
{
    public class GeminiResponseMapper
    {
        public SugerenciaIA ADominio(GeminiResponseDto dto)
        {
            return new SugerenciaIA
            {
                PlatosSugeridos = dto.PlatosSugeridos.Select(MapearPlato).ToList()
            };
        }
        
        private PlatoSugeridoIA MapearPlato(PlatoGeminiDto dto)
        {
            return new PlatoSugeridoIA
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                TiempoPreparacion = dto.TiempoPreparacion,
                PorcionesPosibles = dto.PorcionesPosibles,
                IngredientesSugeridosIA = dto.IngredientesSugeridosIA.Select(MapearIngrediente).ToList()
            };
        }

        private IngredienteSugeridoIA MapearIngrediente(IngredienteGeminiDto dto)
        {
            return new IngredienteSugeridoIA
            {
                InsumoId = dto.InsumoId,
                Nombre = dto.Nombre,
                Cantidad = dto.Cantidad
            };
        }

    }
}

