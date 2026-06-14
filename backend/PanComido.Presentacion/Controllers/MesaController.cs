using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Mesas;
using PanComido.Presentacion.Hubs;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

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
      private readonly IHubContext<PanComidoHub> _hubContext;

      private readonly MesaMapper _mapper;
      private readonly DatosBienvenidaMesaMapper _datosBienvenidaMesaMapper;


        public MesaController(
          OcuparMesaCasoDeUso ocuparMesaCasoDeUso, 
          ListarMesasCasoDeUso listar, 
          GuardarMapaCasoDeUso guardarMapaCasoDeUso,
          CambiarEstadoMesaCasoDeUso cambiarEstadoMesaCasoDeUso,
          ObtenerDatosMesaBienvenidaCasoDeUso obtenerDatosMesaBienvenidaCasoDeUso,
          MesaMapper mapper,
          DatosBienvenidaMesaMapper datosBienvenidaMesaMapper,
          IHubContext<PanComidoHub> hubContext)
      {
         _ocuparMesaCasoDeUso = ocuparMesaCasoDeUso;
         _listarMesas = listar; 
         _guardarMapaCasoDeUso = guardarMapaCasoDeUso;
         _cambiarEstadoMesaCasoDeUso = cambiarEstadoMesaCasoDeUso;
         _obtenerDatosMesaBienvenidaCasoDeUso = obtenerDatosMesaBienvenidaCasoDeUso;
         _hubContext = hubContext;

        _mapper = mapper;
        _datosBienvenidaMesaMapper = datosBienvenidaMesaMapper;
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

        [HttpGet("{idMesa}/comensal/{restauranteId}/bienvenida")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerDatosBienvenidaQR(int idMesa, int restauranteId)
        {
            BienvenidaMesaDatos datosBienvenidaDominio = await _obtenerDatosMesaBienvenidaCasoDeUso.EjecutarAsync(idMesa, restauranteId);
            return Ok(_datosBienvenidaMesaMapper.aDto(datosBienvenidaDominio));
        }

        [HttpPost("comensal/{restauranteId}/{idMesa}/ocupar")]
        [AllowAnonymous]
        public async Task<IActionResult> Ocupar(int restauranteId, int idMesa, [FromBody] OcuparMesaRequestDto request)
        {
            var mesaConPosiciones = await _ocuparMesaCasoDeUso.EjecutarAsync(restauranteId, idMesa, request.CantidadComensales.Value);

            return StatusCode(201,
                new OcuparMesaResponseDto
                {
                    Mesa = _mapper.aDto(mesaConPosiciones),
                    IdComandaGenerada = mesaConPosiciones.idComanda.Value
                });
        }

        [HttpPost("{idMesa}/ocupar")]
        [Authorize(Roles = "Mozo, Gerente")]
        public async Task<IActionResult> Ocupar(int idMesa, [FromBody] OcuparMesaRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var mesaConPosiciones = await _ocuparMesaCasoDeUso.EjecutarAsync(restauranteId, idMesa, request.CantidadComensales.Value);

            return StatusCode(201,
                new OcuparMesaResponseDto {
                    Mesa = _mapper.aDto(mesaConPosiciones),
                    IdComandaGenerada = mesaConPosiciones.idComanda.Value
                });
        }

        [HttpPatch("{id}/estado")]
      [Authorize(Roles = "Mozo, Gerente")]

      public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoMesaRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            if (!Enum.TryParse<PanComido.Dominio.Entidades.Enums.EstadoMesa>(request.EstadoMesa, true, out var estado))
            {
                return BadRequest(new { error = "Estado de mesa inválido." });
            }
            
            var mesaActualizada = await _cambiarEstadoMesaCasoDeUso.EjecutarAsync(restauranteId, id, estado);
            var mesaDto = _mapper.aDto(mesaActualizada);

            await _hubContext.Clients.Group($"Gerente_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
            await _hubContext.Clients.Group($"Mozos_{restauranteId}").SendAsync("MesaActualizada", mesaDto);

            return Ok(mesaDto);
        }
    }
}
