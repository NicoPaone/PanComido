using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.LoteCasosDeUso;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.Lote;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;
using System;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("/lote")]
    [ApiController]
    [Authorize]
    public class LoteController : ControllerBase
    {
        private readonly LoteMapper _loteMapper;

        public LoteController(LoteMapper loteMapper)
        {
            _loteMapper = loteMapper;
        }

        [HttpPost("crear")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromBody] CrearLoteDto dto, [FromServices] CrearLoteCasoDeUso crearLoteCasoDeUso)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var loteCreado = await crearLoteCasoDeUso.EjecutarAsync(
                restauranteId,
                dto.InsumoId,
                dto.Cantidad,
                dto.FechaVencimiento,
                dto.BodegaId
            );

            return StatusCode(201, new
            {
                mensaje = "Lote creado exitosamente.",
                lote = _loteMapper.aDto(loteCreado)
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Modificar(int id, [FromBody] ModificarLoteDto dto, [FromServices] ModificarLoteCasoDeUso modificarLoteCasoDeUso)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var resultado = await modificarLoteCasoDeUso.EjecutarAsync(
                restauranteId,
                id,
                dto.InsumoId,
                dto.Cantidad,
                dto.FechaVencimiento,
                dto.BodegaId
            );

            if (!resultado)
            {
                return NotFound(new ErrorResponseDto { Error = "Lote no encontrado." });
            }

            return Ok(new { Mensaje = "Lote modificado exitosamente." });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int id, [FromServices] EliminarLoteCasoDeUso eliminarLoteCasoDeUso)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var resultado = await eliminarLoteCasoDeUso.EjecutarAsync(restauranteId, id);

            if (!resultado)
            {
                return NotFound(new ErrorResponseDto { Error = "Lote no encontrado." });
            }

            return Ok(new { Mensaje = "Lote eliminado exitosamente." });
        }
    }
}
