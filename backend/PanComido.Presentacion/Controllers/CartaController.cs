using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Presentacion.DTOs.Carta;
using PanComido.Presentacion.DTOs.ErrorResponse;
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
        private readonly ModificarArticuloCasoDeUso _modificarArticuloCasoDeUso;
        private readonly ObtenerCartaComensalCasoDeUso _obtenerCartaComensalCasoDeUso;

        private readonly ArticuloCartaMapper _mapper;
        private readonly CartaComensalMapper _cartaComensalMapper;

        public CartaController(ObtenerArticulosParaCrearCartaCasoDeUso obtenerArticulosCasoDeUso, ModificarArticuloCasoDeUso modificarArticuloCasoDeUso, ArticuloCartaMapper mapper, ObtenerCartaComensalCasoDeUso obtenerCartaComensalCasoDeUso, CartaComensalMapper cartaComensalMapper)
        {
            _obtenerArticulosCasoDeUso = obtenerArticulosCasoDeUso;
            _modificarArticuloCasoDeUso = modificarArticuloCasoDeUso;
            _obtenerCartaComensalCasoDeUso = obtenerCartaComensalCasoDeUso;
            _mapper = mapper;
            _cartaComensalMapper = cartaComensalMapper;
        }

        [HttpGet("{restauranteId}/comensal")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerCartaParaComensal(int restauranteId)
        {
            var articulosDisponibles = await _obtenerCartaComensalCasoDeUso.EjecutarAsync(restauranteId);

            var respuestaJson = _cartaComensalMapper.ParaDtoList(articulosDisponibles);

            return Ok(respuestaJson);
        }

        [HttpGet("mozo")]
        [Authorize(Roles = "Mozo")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerCartaParaMozo()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var articulos = await _obtenerCartaComensalCasoDeUso.EjecutarAsync(restauranteId);
            return Ok(_cartaComensalMapper.ParaDtoList(articulos));
        }

        [HttpGet("obtener-articulos")]
        [Authorize(Roles = "Gerente")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerArticulosParaCarta()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var articulosDominio = await _obtenerArticulosCasoDeUso.EjecutarAsync(restauranteId);

            return Ok(_mapper.aListaDto(articulosDominio));
        }

        [HttpPatch("articulos/{id}")]
        [Authorize(Roles = "Gerente")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]

        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModificarArticulo(int id, [FromBody] ModificarArticuloRequestDto request)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            await _modificarArticuloCasoDeUso.EjecutarAsync(restauranteId, id, request.VisibleEnCarta, request.Destacado);

            return Ok(new { mensaje = "Artículo actualizado exitosamente" });
        }
    }
}