using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Presentacion.DTOs.Mesas;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("mesa")]
    [ApiController]
    public class MesaController : ControllerBase
    {
      private readonly OcuparMesaCasoDeUso _ocuparMesaCasoDeUso;
      private readonly ListarMesasCasoDeUso _listarMesas;
      private readonly MesaMapper _mapper;
      private readonly GuardarMapaCasoDeUso _guardarMapaCasoDeUso;

      public MesaController(
          OcuparMesaCasoDeUso ocuparMesaCasoDeUso, 
          ListarMesasCasoDeUso listar, 
          MesaMapper mapper,
          GuardarMapaCasoDeUso guardarMapaCasoDeUso)
      {
         _ocuparMesaCasoDeUso = ocuparMesaCasoDeUso;
         _listarMesas = listar; 
         _mapper = mapper;
         _guardarMapaCasoDeUso = guardarMapaCasoDeUso;
      }

      [HttpGet]
      public async Task<ActionResult<List<MesaResponseDto>>> ObtenerTodas()
      {
         int restauranteId = HttpContext.ObtenerRestauranteId();
         var mesas = await _listarMesas.EjecutarAsync(restauranteId);
         return Ok(_mapper.aListaDto(mesas));
      }

      [HttpPut("mapa")]
      public async Task<IActionResult> GuardarMapa([FromBody] List<GuardarMesaRequestDto> request)
      {
          int restauranteId = HttpContext.ObtenerRestauranteId();

          var mesasDominio = _mapper.aListaDominio(request);

          await _guardarMapaCasoDeUso.EjecutarAsync(restauranteId, mesasDominio);

          return Ok(new { mensaje = "Mapa de mesas guardado correctamente." });
      }
      [HttpPost("{id}/ocupar")]
        public async Task<IActionResult> Ocupar(int id, [FromBody] OcuparMesaRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

                var mesaConPosiciones = await _ocuparMesaCasoDeUso.EjecutarAsync(restauranteId, id, request.CantidadComensales.Value);

                return StatusCode(201, _mapper.aDto(mesaConPosiciones) );
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Error interno del servidor." });
            }
        }
    }
}
