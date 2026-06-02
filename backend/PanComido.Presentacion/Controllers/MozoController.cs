using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("mozo")]
    [ApiController]
    public class MozoController : ControllerBase
    {
        private readonly ListarComandasActivasMozoCasoDeUso _listarComandasActivasMozoCasoDeUso;
        private readonly ComandaMapper _comandaMapper;

        public MozoController(ListarComandasActivasMozoCasoDeUso listarComandasActivasMozoCasoDeUso, ComandaMapper comandaMapper)
        {
            _listarComandasActivasMozoCasoDeUso = listarComandasActivasMozoCasoDeUso;
            _comandaMapper = comandaMapper;
        }

        [HttpGet("/listar-comandas")]
        public async Task<ActionResult<List<ComandaResponseDto>>> ObtenerComandasActivas()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            int mozoId = HttpContext.ObtenerMozoId();
            var comandas = await _listarComandasActivasMozoCasoDeUso.EjecutarAsync(restauranteId, mozoId);
            var comandasDto = _comandaMapper.ComandaResponseDtoList(comandas);
            return Ok(comandasDto);
        }
    }
}
