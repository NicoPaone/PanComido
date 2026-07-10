using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.Avisos;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("categoria-insumo")]
    [ApiController]
   [Authorize]
   public class CategoriaInsumoController : ControllerBase
    {
        private readonly ListarCategoriasDeInsumosCasoDeUso _listarCategoriasUseCase;
        private readonly CategoriaInsumoMapper _mapper;

        public CategoriaInsumoController(
            ListarCategoriasDeInsumosCasoDeUso listarCategoriasUseCase,
            CategoriaInsumoMapper mapper)
        {
            _listarCategoriasUseCase = listarCategoriasUseCase;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CategoriaInsumoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CategoriaInsumoResponseDto>>> obtener()
        {

            var categoriasDominio = await _listarCategoriasUseCase.EjecutarAsync();

            return Ok(_mapper.aListaDto(categoriasDominio));
        }
    }
}
