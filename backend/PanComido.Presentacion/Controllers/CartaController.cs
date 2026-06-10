using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Presentacion.DTOs.Carta;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("carta")]
    [ApiController]
    [Authorize]

    public class CartaController : ControllerBase
    {
        private readonly ObtenerArticulosParaCrearCartaCasoDeUso _obtenerArticulosCasoDeUso;
        private readonly ArticuloCartaMapper _mapper;
        private readonly ModificarArticuloCasoDeUso _modificarArticuloCasoDeUso;
        public CartaController(ObtenerArticulosParaCrearCartaCasoDeUso obtenerArticulosCasoDeUso, ModificarArticuloCasoDeUso modificarArticuloCasoDeUso, ArticuloCartaMapper mapper)
        {
            _obtenerArticulosCasoDeUso = obtenerArticulosCasoDeUso;
            _modificarArticuloCasoDeUso = modificarArticuloCasoDeUso;
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

        [HttpPatch("articulos/{id}")]
        [Authorize(Roles = "Gerente")]

        public async Task<IActionResult> ModificarArticulo(int id, [FromBody] ModificarArticuloRequestDto request)
        {
            // 3. Usamos el método de tu compañero para obtener el ID limpio
            var restauranteId = HttpContext.ObtenerRestauranteId();

            // 4. Ahora sí, la variable existe y funciona perfecto
            await _modificarArticuloCasoDeUso.EjecutarAsync(restauranteId, id, request.VisibleEnCarta, request.Destacado);

            return Ok(new { mensaje = "Artículo actualizado exitosamente" });
        }




    }
}