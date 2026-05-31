using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("insumo")]
    [ApiController]
    public class InsumoController : ControllerBase
    {
        private readonly ListarInsumoCasoDeUso _listarInsumoCasoDeUso;
        private readonly CrearInsumoCasoDeUso _crearInsumoCasoDeUso;
        private readonly InsumoMapper _mapper;

        public InsumoController(ListarInsumoCasoDeUso listarInsumoCasoDeUso, CrearInsumoCasoDeUso crearInsumoCasoDeUso, InsumoMapper mapper)
        {
            _listarInsumoCasoDeUso = listarInsumoCasoDeUso;
            _crearInsumoCasoDeUso = crearInsumoCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<InsumoResponseDto>>> obtener() {

            var restauranteId = HttpContext.ObtenerRestauranteId();

            var insumos = await _listarInsumoCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _mapper.aListaDto(insumos);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearInsumoRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

                Insumo insumoDominio = _mapper.aDominio(request);

                await _crearInsumoCasoDeUso.EjecutarAsync(
                    restauranteId,
                    insumoDominio,
                    request.CantidadInicial,
                    request.BodegaId,
                    request.FechaVencimiento
                );

                return StatusCode(201, new { mensaje = "Insumo creado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno al intentar crear el insumo." });
            }
        }




    }
}
