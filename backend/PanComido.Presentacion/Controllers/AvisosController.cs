using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.AvisosCasosDeUso;
using PanComido.Presentacion.DTOs.Avisos;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [ApiController]
    [Route("[avisos]")]
    public class AvisosController : ControllerBase
    {
        private readonly ListarInsumosConStockCriticoCasoDeUso _listarInsumosConStockCriticoCasoDeUso;
        private readonly ListarInsumosConVencimientoProximoCasoDeUso _listarInsumosConVencimientoProximoCasoDeUso;
        private readonly InsumoMapper _insumoMapper;
        private readonly LoteMapper _loteMapper;

        public AvisosController(ListarInsumosConStockCriticoCasoDeUso listarInsumosConStockCriticoCasoDeUso,
                                ListarInsumosConVencimientoProximoCasoDeUso listarInsumosConVencimientoProximoCasoDeUso,
                                InsumoMapper insumoMapper,
                                LoteMapper loteMapper)
        {
            _listarInsumosConStockCriticoCasoDeUso = listarInsumosConStockCriticoCasoDeUso;
            _listarInsumosConVencimientoProximoCasoDeUso = listarInsumosConVencimientoProximoCasoDeUso;
            _insumoMapper = insumoMapper;
            _loteMapper = loteMapper;
        }

        [HttpGet("/avisos")]
        public async Task<ActionResult<AvisosResponseDto>> obtener()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var insumosConStockCritico = await _listarInsumosConStockCriticoCasoDeUso.EjecutarAsync(restauranteId);
            var insumosConVencimientoProximo = await _listarInsumosConVencimientoProximoCasoDeUso.EjecutarAsync(restauranteId);

            var response = new AvisosResponseDto
            {
                InsumosConStockCritico = _insumoMapper.aListaDto(insumosConStockCritico),
                InsumosConVencimientoProximo = _loteMapper.aDiccionarioDto(insumosConVencimientoProximo)
            };

            return Ok(response);
        }
    }
}
