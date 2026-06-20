using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.UnidadMedidaCasosDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("unidad-medida")]
    [ApiController]
    public class UnidadMedidaController : ControllerBase
    {
        private readonly ListarUnidadesDeMedidaCasoDeUso _listarUnidadesDeMedidaUseCase;
        private readonly UnidadMedidaMapper _mapper;

        public UnidadMedidaController(
            ListarUnidadesDeMedidaCasoDeUso listarUnidadesDeMedidaUseCase,
            UnidadMedidaMapper mapper)
        {
            _listarUnidadesDeMedidaUseCase = listarUnidadesDeMedidaUseCase;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<UnidadMedidaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UnidadMedidaResponseDto>>> obtener()
        {
            var unidadesDeMedidaDominio = await _listarUnidadesDeMedidaUseCase.EjecutarAsync();

            return Ok(_mapper.aListaDto(unidadesDeMedidaDominio));
        }
    }
}
