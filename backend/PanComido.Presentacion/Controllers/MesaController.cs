using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.Mesas;
using PanComido.Presentacion.Hubs;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{


    namespace PanComido.Presentacion.Controllers
    {
        [Route("mesa")]
        [ApiController]
        [Authorize]

        public class MesaController : ControllerBase
        {
            private readonly OcuparMesaCasoDeUso _ocuparMesaCasoDeUso;
            private readonly ListarMesasCasoDeUso _listarMesas;
            private readonly GuardarMapaCasoDeUso _guardarMapaCasoDeUso;
            private readonly CambiarEstadoMesaCasoDeUso _cambiarEstadoMesaCasoDeUso;
            private readonly ObtenerDatosMesaBienvenidaCasoDeUso _obtenerDatosMesaBienvenidaCasoDeUso;
            private readonly AsignarMozosMesaCasoDeUso _asignarMozosMesaCasoDeUso;
            private readonly DesasignarMozoMesaCasoDeUso _desasignarMozoMesaCasoDeUso;
            private readonly ListarMozosParaMesaCasoDeUso _listarMozosParaMesaCasoDeUso;
            private readonly IHubContext<PanComidoHub> _hubContext;

            private readonly MesaMapper _mapper;
            private readonly DatosBienvenidaMesaMapper _datosBienvenidaMesaMapper;


            public MesaController(
              OcuparMesaCasoDeUso ocuparMesaCasoDeUso,
              ListarMesasCasoDeUso listar,
              GuardarMapaCasoDeUso guardarMapaCasoDeUso,
              CambiarEstadoMesaCasoDeUso cambiarEstadoMesaCasoDeUso,
              ObtenerDatosMesaBienvenidaCasoDeUso obtenerDatosMesaBienvenidaCasoDeUso,
              AsignarMozosMesaCasoDeUso asignarMozosMesaCasoDeUso,
              DesasignarMozoMesaCasoDeUso desasignarMozoMesaCasoDeUso,
              ListarMozosParaMesaCasoDeUso listarMozosParaMesaCasoDeUso,
              MesaMapper mapper,
              DatosBienvenidaMesaMapper datosBienvenidaMesaMapper,
              IHubContext<PanComidoHub> hubContext)
            {
                _ocuparMesaCasoDeUso = ocuparMesaCasoDeUso;
                _listarMesas = listar;
                _guardarMapaCasoDeUso = guardarMapaCasoDeUso;
                _cambiarEstadoMesaCasoDeUso = cambiarEstadoMesaCasoDeUso;
                _obtenerDatosMesaBienvenidaCasoDeUso = obtenerDatosMesaBienvenidaCasoDeUso;
                _asignarMozosMesaCasoDeUso = asignarMozosMesaCasoDeUso;
                _desasignarMozoMesaCasoDeUso = desasignarMozoMesaCasoDeUso;
                _listarMozosParaMesaCasoDeUso = listarMozosParaMesaCasoDeUso;
                _hubContext = hubContext;

                _mapper = mapper;
                _datosBienvenidaMesaMapper = datosBienvenidaMesaMapper;
            }

            [HttpGet]
            [ProducesResponseType(typeof(List<MesaResponseDto>), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<List<MesaResponseDto>>> ObtenerTodas()
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();
                var mesas = await _listarMesas.EjecutarAsync(restauranteId);
                return Ok(_mapper.aListaDto(mesas));
            }

            [HttpGet("mozos")]
            [Authorize(Roles = "Gerente, Mozo")]
            public async Task<IActionResult> ObtenerMozosParaMesa()
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();
                var mozos = await _listarMozosParaMesaCasoDeUso.EjecutarAsync(restauranteId);
                return Ok(mozos.Select(m => new { id = m.Id, nombre = m.Nombre }));
            }

            [HttpPut("mapa")]
            [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> GuardarMapa([FromBody] List<GuardarMesaRequestDto> request)
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

                var mesasDominio = _mapper.aListaDominio(request);

                await _guardarMapaCasoDeUso.EjecutarAsync(restauranteId, mesasDominio);

                return Ok(new { mensaje = "Mapa de mesas guardado correctamente." });
            }

            [HttpGet("{idMesa}/comensal/{restauranteId}/bienvenida")]
            [AllowAnonymous]
            public async Task<IActionResult> ObtenerDatosBienvenidaQR(int idMesa, int restauranteId)
            {
                BienvenidaMesaDatos datosBienvenidaDominio = await _obtenerDatosMesaBienvenidaCasoDeUso.EjecutarAsync(idMesa, restauranteId);
                return Ok(_datosBienvenidaMesaMapper.aDto(datosBienvenidaDominio));
            }

        [HttpPost("comensal/{restauranteId}/{idMesa}/ocupar")]
        [AllowAnonymous]
            [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> Ocupar(int restauranteId, int idMesa, [FromBody] OcuparMesaRequestDto request)
        {
            var mesaConPosiciones = await _ocuparMesaCasoDeUso.EjecutarAsync(restauranteId, idMesa, request.CantidadComensales.Value);

                var mesaDto = _mapper.aDto(mesaConPosiciones);
                await _hubContext.Clients.Group($"Gerente_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
                await _hubContext.Clients.Group($"Mozos_{restauranteId}").SendAsync("MesaActualizada", mesaDto);

                return StatusCode(201,
                    new OcuparMesaComensalResponseDto
                    {
                        Mesa = _mapper.aMesaSinPosicionesResponseDto(mesaConPosiciones),
                        IdComandaGenerada = mesaConPosiciones.idComanda.Value
                    });
            }

            [HttpPost("{idMesa}/ocupar")]
            [Authorize(Roles = "Mozo, Gerente")]
            [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> Ocupar(int idMesa, [FromBody] OcuparMesaRequestDto request)
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

            var mesaConPosiciones = await _ocuparMesaCasoDeUso.EjecutarAsync(restauranteId, idMesa, request.CantidadComensales.Value);

                var mesaDto = _mapper.aDto(mesaConPosiciones);
                await _hubContext.Clients.Group($"Gerente_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
                await _hubContext.Clients.Group($"Mozos_{restauranteId}").SendAsync("MesaActualizada", mesaDto);

                return StatusCode(201,
                    new OcuparMesaResponseDto
                    {
                        Mesa = mesaDto,
                        IdComandaGenerada = mesaConPosiciones.idComanda.Value
                    });
            }

            [HttpPatch("{id}/estado")]
            [Authorize(Roles = "Mozo, Gerente")]
            [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoMesaRequestDto request)
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();
                if (!Enum.TryParse<Dominio.Entidades.Enums.EstadoMesa>(request.EstadoMesa, true, out var estado))
                {
                    return BadRequest(new { error = "Estado de mesa inválido." });
                }

                var mesaActualizada = await _cambiarEstadoMesaCasoDeUso.EjecutarAsync(restauranteId, id, estado);
                var mesaDto = _mapper.aDto(mesaActualizada);

                await _hubContext.Clients.Group($"Gerente_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
                await _hubContext.Clients.Group($"Mozos_{restauranteId}").SendAsync("MesaActualizada", mesaDto);

                return Ok(mesaDto);
            }

            [HttpPost("{id}/mozos")]
            [Authorize(Roles = "Gerente")]
            public async Task<IActionResult> AsignarMozos(int id, [FromBody] List<int> mozosIds)
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

                await _asignarMozosMesaCasoDeUso.EjecutarAsync(restauranteId, id, mozosIds);

                var mesas = await _listarMesas.EjecutarAsync(restauranteId);
                var mesaActualizada = mesas.FirstOrDefault(m => m.Id == id);
                if (mesaActualizada != null)
                {
                    var mesaDto = _mapper.aDto(mesaActualizada);
                    await _hubContext.Clients.Group($"Gerente_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
                    await _hubContext.Clients.Group($"Mozos_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
                }

                return Ok(new { mensaje = "Mozos asignados correctamente." });
            }

            [HttpDelete("{id}/mozos/{mozoId}")]
            [Authorize(Roles = "Gerente")]
            public async Task<IActionResult> DesasignarMozo(int id, int mozoId)
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

                await _desasignarMozoMesaCasoDeUso.EjecutarAsync(restauranteId, id, mozoId);

                var mesas = await _listarMesas.EjecutarAsync(restauranteId);
                var mesaActualizada = mesas.FirstOrDefault(m => m.Id == id);
                if (mesaActualizada != null)
                {
                    var mesaDto = _mapper.aDto(mesaActualizada);
                    await _hubContext.Clients.Group($"Gerente_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
                    await _hubContext.Clients.Group($"Mozos_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
                }

                return Ok(new { mensaje = "Mozo desasignado correctamente." });
            }
        }
    }
}
