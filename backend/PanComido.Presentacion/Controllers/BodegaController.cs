using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Bodegas;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("bodega")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class BodegaController : ControllerBase
    {
        private readonly ListarBodegasConInsumosCasoDeUso _listarBodegasConInsumosCasoDeUso;
        private readonly ListarBodegasCasoDeUso _listarBodegasCasoDeUso;
        private readonly BodegaMapper _bodegaMapper;
        private readonly CrearBodegaCasoDeUso _crearBodegaCasoDeUso;
        private readonly ModificarBodegaCasoDeUso _modificarBodegaCasoDeUso;
        private readonly EliminarBodegaCasoDeUso _eliminarBodegaCasoDeUso;
        public BodegaController(
            ListarBodegasConInsumosCasoDeUso listarBodegasConInsumosCasoDeUso,
            ListarBodegasCasoDeUso listarBodegasCasoDeUso,
            BodegaMapper bodegaMapper,
            CrearBodegaCasoDeUso crearBodegaCasoDeUso,
            ModificarBodegaCasoDeUso modificarBodegaCasoDeUso,
            EliminarBodegaCasoDeUso eliminarBodegaCasoDeUso)
        {
            _listarBodegasConInsumosCasoDeUso = listarBodegasConInsumosCasoDeUso;
            _listarBodegasCasoDeUso = listarBodegasCasoDeUso;
            _bodegaMapper = bodegaMapper;
            _crearBodegaCasoDeUso = crearBodegaCasoDeUso;
            _modificarBodegaCasoDeUso = modificarBodegaCasoDeUso;
            _eliminarBodegaCasoDeUso = eliminarBodegaCasoDeUso;
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> obtener()
        {
            List<Bodega> bodegas = await _listarBodegasCasoDeUso.EjecutarAsync(HttpContext.ObtenerRestauranteId());
            return Ok(_bodegaMapper.bodegasAListaDto(bodegas));
        }

        [HttpGet("con-insumos")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> obtenerConInsumos()
        {
            List<Bodega> bodegasConInsumos = await _listarBodegasConInsumosCasoDeUso.EjecutarAsync(HttpContext.ObtenerRestauranteId());
            return Ok(_bodegaMapper.bodegasConInsumosAListaDto(bodegasConInsumos));
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromBody] GuardarBodegaRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            Bodega bodegaDominio = _bodegaMapper.aDominio(request);
            Bodega creado = await _crearBodegaCasoDeUso.EjecutarAsync(bodegaDominio, restauranteId);
            return StatusCode(201, new
            {
                mensaje = "Bodega creada correctamente.",
                bodega = _bodegaMapper.bodegaADto(creado)
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Modificar(int id, [FromBody] GuardarBodegaRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            Bodega bodegaDominio = _bodegaMapper.aDominio(request, id);
            Bodega modificado = await _modificarBodegaCasoDeUso.EjecutarAsync(bodegaDominio, restauranteId);
            return Ok(new
            {
                mensaje = "Bodega modificada correctamente.",
                bodega = _bodegaMapper.bodegaADto(modificado)
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await _eliminarBodegaCasoDeUso.EjecutarAsync(id, restauranteId);
            return Ok(new { mensaje = "Bodega eliminada correctamente." });
        }



    }
}
