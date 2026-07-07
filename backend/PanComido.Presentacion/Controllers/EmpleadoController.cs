using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Empleado;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("empleado")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class EmpleadoController : ControllerBase
    {
        private readonly ListarEmpleadosCasoDeUso _listarEmpleadosCasoDeUso;
        private readonly CrearEmpleadoCasoDeUso _crearEmpleadoCasoDeUso;
        private readonly ModificarEmpleadoCasoDeUso _modificarEmpleadoCasoDeUso;
        private readonly EliminarEmpleadoCasoDeUso _eliminarEmpleadoCasoDeUso;
        private readonly EmpleadoMapper _mapper;

        public EmpleadoController(
            ListarEmpleadosCasoDeUso listarEmpleadosCasoDeUso,
            CrearEmpleadoCasoDeUso crearEmpleadoCasoDeUso,
            ModificarEmpleadoCasoDeUso modificarEmpleadoCasoDeUso,
            EliminarEmpleadoCasoDeUso eliminarEmpleadoCasoDeUso,
            EmpleadoMapper mapper)
        {
            _listarEmpleadosCasoDeUso = listarEmpleadosCasoDeUso;
            _crearEmpleadoCasoDeUso = crearEmpleadoCasoDeUso;
            _modificarEmpleadoCasoDeUso = modificarEmpleadoCasoDeUso;
            _eliminarEmpleadoCasoDeUso = eliminarEmpleadoCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<EmpleadoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EmpleadoResponseDto>>> ObtenerTodos()
        {
            try
            {
                var restauranteId = HttpContext.ObtenerRestauranteId();
                var empleados = await _listarEmpleadosCasoDeUso.EjecutarAsync(restauranteId);
                var dtos = _mapper.aListaDto(empleados);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromBody] CrearEmpleadoRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var restauranteId = HttpContext.ObtenerRestauranteId();
                var empleadoDominio = _mapper.aDominio(request);

                var creado = await _crearEmpleadoCasoDeUso.EjecutarAsync(
                    restauranteId,
                    empleadoDominio,
                    request.Contrasenia,
                    request.TurnosIds
                );

                return StatusCode(201, new
                {
                    mensaje = "Empleado creado correctamente.",
                    empleado = _mapper.aDto(creado)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponseDto { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Modificar(int id, [FromBody] ModificarEmpleadoRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var restauranteId = HttpContext.ObtenerRestauranteId();
                var empleadoDominio = _mapper.aDominio(request, id);

                var modificado = await _modificarEmpleadoCasoDeUso.EjecutarAsync(
                    restauranteId,
                    empleadoDominio,
                    request.Contrasenia,
                    request.TurnosIds
                );

                return Ok(new
                {
                    mensaje = "Empleado modificado correctamente.",
                    empleado = _mapper.aDto(modificado)
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponseDto { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponseDto { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var restauranteId = HttpContext.ObtenerRestauranteId();
                await _eliminarEmpleadoCasoDeUso.EjecutarAsync(id, restauranteId);

                return Ok(new { mensaje = "Empleado eliminado correctamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponseDto { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = ex.Message });
            }
        }
    }
}
