using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ArticuloCasosDeUso;
using PanComido.Dominio.Entidades;
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

        [HttpGet("{id}/comensal")]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            try
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

                Articulo articuloDominio = await _obtenerDetalleArticuloCasoDeUso.EjecutarAsync(restauranteId, id);

                return Ok(_mapper.aDto(articuloDominio));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }
    }
}
