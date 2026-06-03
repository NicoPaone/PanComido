using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Presentacion.Mappers;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("carta")]
    [ApiController]
    public class CartaController : ControllerBase
    {
        private readonly ObtenerArticulosParaCrearCartaCasoDeUso _obtenerArticulosCasoDeUso;
        private readonly ArticuloCartaMapper _mapper;

        public CartaController(ObtenerArticulosParaCrearCartaCasoDeUso obtenerArticulosCasoDeUso, ArticuloCartaMapper mapper)
        {
            _obtenerArticulosCasoDeUso = obtenerArticulosCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet("obtener-articulos")]
        public async Task<IActionResult> ObtenerArticulosParaCarta()
        {
            // Ejecutamos el caso de uso que trae la lista y hace la matemática
            var articulosDominio = await _obtenerArticulosCasoDeUso.EjecutarAsync();

            // Usamos nuestro mapper para traducirlo al DTO de Angular y devolvemos 200 OK
            return Ok(_mapper.aListaDto(articulosDominio));
        }
    }
}