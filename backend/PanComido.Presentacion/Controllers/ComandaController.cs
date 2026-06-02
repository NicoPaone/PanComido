using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Presentacion.DTOs.Comanda;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("comanda")]
    [ApiController]
    public class ComandaController : ControllerBase
    {
        private readonly ListarComandaActivaCocinaCasoDeUso _listarComandasActivasCocinaCasoDeUso;
        private readonly ModificarEstadoComandaCasoDeUso _modificarEstadoComandaCasoDeUso;
        private readonly MarcarItemsEntregadosCasoDeUso _marcarItemsEntregadosCasoDeUso;
        private readonly ComandaMapper _mapper;

        public ComandaController(
            ListarComandaActivaCocinaCasoDeUso listarComandaActivasCasoDeUso,
            ModificarEstadoComandaCasoDeUso modificar,
            MarcarItemsEntregadosCasoDeUso marcarItemsEntregadosCasoDeUso,
            ComandaMapper mapper)
        {
            _listarComandasActivasCocinaCasoDeUso = listarComandaActivasCasoDeUso;
            _modificarEstadoComandaCasoDeUso = modificar;
            _marcarItemsEntregadosCasoDeUso = marcarItemsEntregadosCasoDeUso;
            _mapper = mapper;

        }

        [HttpGet("activas")]
        public async Task<ActionResult<List<ComandaResponseDto>>> ObtenerComandasActivas()
        {
            int restauranteId = 1;
            var comandas = await _listarComandasActivasCocinaCasoDeUso.Ejecutar(restauranteId);
            var comandasDto = _mapper.ComandaResponseDtoList(comandas);
            return Ok(comandasDto);

        }
        [HttpPut("activas/{mesaId}/{estadoId}")]
        public async Task<ActionResult<ComandaResponseDto>> ModificarEstadoDeComanda(int mesaId, int estadoId)
        {
            var comanda = await _modificarEstadoComandaCasoDeUso.EjecutarAsync(mesaId, estadoId);
            var comandaDto = _mapper.ComandaResponseDto(comanda);
            return Ok(comandaDto);
        }

        [HttpPut("{comandaId}/entregar-items")]
        public async Task<ActionResult<ComandaResponseDto>> MarcarItemComandaEntregado(int comandaId, [FromBody] List<int> itemsEntregados)
        {
            var comanda = await _marcarItemsEntregadosCasoDeUso.EjecutarAsync(comandaId, itemsEntregados);
            var comandaDto = _mapper.ComandaResponseDto(comanda);
            return Ok(comandaDto);
        } 
    }
}
