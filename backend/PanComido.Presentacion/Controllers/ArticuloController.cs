using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ArticuloCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("articulo")]
    [ApiController]
    public class ArticuloController : ControllerBase
    {
        private readonly ObtenerDetalleArticuloCasoDeUso _obtenerDetalleArticuloCasoDeUso;
        private readonly ArticuloMapper _mapper;

        public ArticuloController(
            ObtenerDetalleArticuloCasoDeUso obtenerDetalleArticuloCasoDeUso,
            ArticuloMapper mapper)
        {
            _obtenerDetalleArticuloCasoDeUso = obtenerDetalleArticuloCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet("{restauranteId}/{id}/comensal")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerDetalle(int restauranteId, int id)
        {
            Articulo articuloDominio = await _obtenerDetalleArticuloCasoDeUso.EjecutarAsync(restauranteId, id);

            return Ok(_mapper.aDto(articuloDominio));
        }

        [HttpGet("{id}/mozo")]
        [Authorize(Roles = "Mozo")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerDetalleMozo(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            Articulo articuloDominio = await _obtenerDetalleArticuloCasoDeUso.EjecutarAsync(restauranteId, id);

            return Ok(_mapper.aDto(articuloDominio));
        }
    }
}
