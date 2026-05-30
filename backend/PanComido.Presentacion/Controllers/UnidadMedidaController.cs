using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.UnidadMedidaCasosDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<List<UnidadMedidaResponseDto>>> obtener()
        {
            var unidadesDeMedidaDominio = await _listarUnidadesDeMedidaUseCase.EjecutarAsync();

            return Ok(_mapper.aListaDto(unidadesDeMedidaDominio));
        }
    }
}
