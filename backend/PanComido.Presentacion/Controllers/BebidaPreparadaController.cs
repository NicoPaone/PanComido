using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso;
using PanComido.Dominio.Constantes;
using PanComido.Presentacion.DTOs.BebidaPreparada;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("/bebida-preparada")]
    [ApiController]
    [Authorize]
    public class BebidaPreparadaController : ControllerBase
    {
        private readonly CrearBebidaPreparadaCasoDeUso _crearBebidaPreparadaCasoDeUso;
        private readonly ModificarBebidaPreparadaCasoDeUso _modificarBebidaPreparadaCasoDeUso;
        private readonly ObtenerBebidaPreparadaPorIdCasoDeUso _obtenerBebidaPreparadaPorIdCasoDeUso;
        private readonly EliminarBebidaPreparadaCasoDeUso _eliminarBebidaPreparadaCasoDeUso;
        private readonly BebidaPreparadaMapper _bebidaPreparadaMapper;

        public BebidaPreparadaController(
            CrearBebidaPreparadaCasoDeUso crearBebidaPreparadaCasoDeUso,
            ModificarBebidaPreparadaCasoDeUso modificarBebidaPreparadaCasoDeUso,
            ObtenerBebidaPreparadaPorIdCasoDeUso obtenerBebidaPreparadaPorIdCasoDeUso,
            EliminarBebidaPreparadaCasoDeUso eliminarBebidaPreparadaCasoDeUso,
            BebidaPreparadaMapper bebidaPreparadaMapper)
        {
            _crearBebidaPreparadaCasoDeUso = crearBebidaPreparadaCasoDeUso;
            _modificarBebidaPreparadaCasoDeUso = modificarBebidaPreparadaCasoDeUso;
            _obtenerBebidaPreparadaPorIdCasoDeUso = obtenerBebidaPreparadaPorIdCasoDeUso;
            _eliminarBebidaPreparadaCasoDeUso = eliminarBebidaPreparadaCasoDeUso;
            _bebidaPreparadaMapper = bebidaPreparadaMapper;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DetalleBebidaPreparadaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerBebidaPreparadaPorId(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var bebidaDominio = await _obtenerBebidaPreparadaPorIdCasoDeUso.EjecutarAsync(id, restauranteId);

            return Ok(_bebidaPreparadaMapper.aDto(bebidaDominio));
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearBebidaPreparada([FromForm] CrearBebidaPreparadaRequestDto request, IFormFile? imagen)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int restauranteId = HttpContext.ObtenerRestauranteId();
            var bebidaDominio = _bebidaPreparadaMapper.aDominio(request);

            Stream? stream = imagen?.OpenReadStream();
            string? nombreArchivo = imagen?.FileName;

            var bebidaCreada = await _crearBebidaPreparadaCasoDeUso.EjecutarAsync(
                restauranteId,
                bebidaDominio,
                RutasCloudinary.MenuPlatos,
                stream,
                nombreArchivo);

            return StatusCode(201, new
            {
                bebidaPreparada = _bebidaPreparadaMapper.aDto(bebidaCreada),
                mensaje = "Bebida preparada creada correctamente."
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModificarBebidaPreparada(int id, [FromForm] ModificarBebidaPreparadaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int restauranteId = HttpContext.ObtenerRestauranteId();
            var bebidaDominio = _bebidaPreparadaMapper.ModificarADominio(id, request);

            Stream? stream = request.Imagen?.OpenReadStream();
            string? nombreArchivo = request.Imagen?.FileName;

            var bebidaModificada = await _modificarBebidaPreparadaCasoDeUso.EjecutarAsync(
                restauranteId,
                bebidaDominio,
                RutasCloudinary.MenuPlatos,
                stream,
                nombreArchivo);

            return Ok(new
            {
                bebidaPreparada = _bebidaPreparadaMapper.aDto(bebidaModificada),
                mensaje = "Bebida preparada modificada correctamente."
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarBebidaPreparada(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await _eliminarBebidaPreparadaCasoDeUso.EjecutarAsync(id, restauranteId);
            return Ok(new { mensaje = "Bebida preparada eliminada correctamente." });
        }
    }
}
