using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("insumo")]
    [ApiController]
   [Authorize(Roles = "Gerente")]

   public class InsumoController : ControllerBase
    {
        private readonly ListarInsumoCasoDeUso _listarInsumoCasoDeUso;
        private readonly CrearInsumoCasoDeUso _crearInsumoCasoDeUso;
        private readonly InsumoMapper _mapper;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly LoteMapper _loteMapper;

        public InsumoController(
            ListarInsumoCasoDeUso listarInsumoCasoDeUso,
            CrearInsumoCasoDeUso crearInsumoCasoDeUso,
            InsumoMapper mapper,
            ILoteRepositorio loteRepositorio,
            LoteMapper loteMapper)
        {
            _listarInsumoCasoDeUso = listarInsumoCasoDeUso;
            _crearInsumoCasoDeUso = crearInsumoCasoDeUso;
            _mapper = mapper;
            _loteRepositorio = loteRepositorio;
            _loteMapper = loteMapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<InsumoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InsumoResponseDto>>> obtener() {

            var restauranteId = HttpContext.ObtenerRestauranteId();

            var insumos = await _listarInsumoCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _mapper.aListaDto(insumos);
            return Ok(dtos);
        }

        [HttpGet("lotes")]
        public async Task<IActionResult> ObtenerLotes()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var lotes = await _loteRepositorio.ObtenerLotesPorRestauranteAsync(restauranteId);
            return Ok(_loteMapper.aListaDto(lotes));
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

                Insumo insumoCreado = await _crearInsumoCasoDeUso.EjecutarAsync(
                    restauranteId,
                    insumoDominio,
                    request.CantidadInicial,
                    request.BodegaId,
                    request.FechaVencimiento
                );

                return StatusCode(201, new {
                    insumo = _mapper.aDto(insumoCreado),
                    mensaje = "Insumo creado correctamente." 
                });
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
