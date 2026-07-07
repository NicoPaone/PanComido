using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.ReglaTiempoExtra;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("configuracion/tiempos-extra")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class ConfiguracionTiemposController : ControllerBase
    {
        private readonly ObtenerReglasTiempoExtraCasoDeUso _obtenerCU;
        private readonly CrearReglaTiempoExtraCasoDeUso _crearCU;
        private readonly ModificarReglaTiempoExtraCasoDeUso _modificarCU;
        private readonly EliminarReglaTiempoExtraCasoDeUso _eliminarCU;
        private readonly ReglaTiempoExtraMapper _mapper;

        public ConfiguracionTiemposController(
            ObtenerReglasTiempoExtraCasoDeUso obtenerCU,
            CrearReglaTiempoExtraCasoDeUso crearCU,
            ModificarReglaTiempoExtraCasoDeUso modificarCU,
            EliminarReglaTiempoExtraCasoDeUso eliminarCU,
            ReglaTiempoExtraMapper mapper)
        {
            _obtenerCU = obtenerCU;
            _crearCU = crearCU;
            _modificarCU = modificarCU;
            _eliminarCU = eliminarCU;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ReglaTiempoExtraResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ReglaTiempoExtraResponseDto>>> ObtenerTodas()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var reglas = await _obtenerCU.EjecutarAsync(restauranteId);
            return Ok(_mapper.aListaDto(reglas));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReglaTiempoExtraResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReglaTiempoExtraResponseDto>> Crear([FromBody] GuardarReglaTiempoExtraRequestDto dto)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            ReglaTiempoExtra reglaCreada = await _crearCU.EjecutarAsync(_mapper.aDominio(dto, restauranteId));
            return Created("La regla fue creada correctamente", _mapper.aDto(reglaCreada));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ReglaTiempoExtraResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReglaTiempoExtraResponseDto>> Modificar(int id, [FromBody] GuardarReglaTiempoExtraRequestDto dto)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            ReglaTiempoExtra reglaModificada = await _modificarCU.EjecutarAsync(id, _mapper.aDominio(dto, restauranteId));
            return Ok(_mapper.aDto(reglaModificada));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await _eliminarCU.EjecutarAsync(id, restauranteId);
            return Ok("La regla fue borrada con exito.");
        }
    }
}
