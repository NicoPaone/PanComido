using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("categoria-insumo")]
    [ApiController]
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
        public async Task<ActionResult<List<CategoriaInsumoResponseDto>>> obtener()
        {

            var categoriasDominio = await _listarCategoriasUseCase.EjecutarAsync();

            return Ok(_mapper.aListaDto(categoriasDominio));
        }
    }
}
