using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Bodegas;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("tipo-bodega")]
    [ApiController]
    [Authorize(Roles = "Gerente, Cocina")]
    public class TipoBodegaController : ControllerBase
    {
        private readonly ListarTiposBodegaCasoDeUso _listarTiposBodegaCasoDeUso;
        private readonly TipoBodegaMapper _mapper;
        
        public TipoBodegaController(ListarTiposBodegaCasoDeUso listarTiposBodegaCasoDeUso, TipoBodegaMapper mapper)
        {
            _listarTiposBodegaCasoDeUso = listarTiposBodegaCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TipoBodegaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerTodos()
        {
            List<TipoBodega> tipos = await _listarTiposBodegaCasoDeUso.EjecutarAsync();
            return Ok(_mapper.aListaDto(tipos));
        }
    }
}
