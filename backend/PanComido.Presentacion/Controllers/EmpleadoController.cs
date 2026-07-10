using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
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
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var empleados = await _listarEmpleadosCasoDeUso.EjecutarAsync(restauranteId);
            var dtos = _mapper.aListaDto(empleados);
            return Ok(dtos);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmpleadoOperacionResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EmpleadoOperacionResponseDto>> Crear([FromBody] CrearEmpleadoRequestDto request)
        {
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

                return StatusCode(StatusCodes.Status201Created, new EmpleadoOperacionResponseDto
                {
                    Mensaje = "Empleado creado correctamente.",
                    Empleado = _mapper.aDto(creado)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "bad_request"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "business_rule_violation"));
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmpleadoOperacionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EmpleadoOperacionResponseDto>> Modificar(int id, [FromBody] ModificarEmpleadoRequestDto request)
        {
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

                return Ok(new EmpleadoOperacionResponseDto
                {
                    Mensaje = "Empleado modificado correctamente.",
                    Empleado = _mapper.aDto(modificado)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "bad_request"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "not_found"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "business_rule_violation"));
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(EliminarEmpleadoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EliminarEmpleadoResponseDto>> Eliminar(int id)
        {
            try
            {
                var restauranteId = HttpContext.ObtenerRestauranteId();
                await _eliminarEmpleadoCasoDeUso.EjecutarAsync(id, restauranteId);

                return Ok(new EliminarEmpleadoResponseDto
                {
                    Mensaje = "Empleado eliminado correctamente."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "not_found"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "business_rule_violation"));
            }
        }
    }
}
