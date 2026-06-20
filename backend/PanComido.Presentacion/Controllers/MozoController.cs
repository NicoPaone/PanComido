using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Presentacion.DTOs.Comanda;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.MetodoDePago;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("mozo")]
    [ApiController]
    [Authorize(Roles = "Mozo")]
    public class MozoController : ControllerBase
    {
        private readonly ListarComandasActivasMozoCasoDeUso _listarComandasActivasMozoCasoDeUso;
        private readonly ComandaMapper _comandaMapper;

        public MozoController(ListarComandasActivasMozoCasoDeUso listarComandasActivasMozoCasoDeUso, ComandaMapper comandaMapper)
        {
            _listarComandasActivasMozoCasoDeUso = listarComandasActivasMozoCasoDeUso;
            _comandaMapper = comandaMapper;
        }

        [HttpGet("listar-comandas")]
        [ProducesResponseType(typeof(List<ComandaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ComandaResponseDto>>> ObtenerComandasActivas()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            int mozoId = HttpContext.ObtenerEmpleadoId();
            var comandas = await _listarComandasActivasMozoCasoDeUso.EjecutarAsync(restauranteId, mozoId);
            var comandasDto = _comandaMapper.ComandaResponseDtoList(comandas);
            return Ok(comandasDto);
        }
    }
}
