using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Presentacion.DTOs.Cliente;
using PanComido.Presentacion.DTOs.Comanda;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("comanda")]
    [ApiController]
    [Authorize]

    public class ComandaController : ControllerBase
    {
        private readonly ListarComandaActivaCocinaCasoDeUso _listarComandasActivasCocinaCasoDeUso;
        private readonly ModificarEstadoComandaCasoDeUso _modificarEstadoComandaCasoDeUso;
        private readonly ConfirmarPedidoClienteAComandaCasoDeUso _confirmarPedidoCasoDeUso;
        private readonly MarcarItemsEntregadosCasoDeUso _marcarItemsEntregadosCasoDeUso;
        private readonly LlamarMozoComandaCasoDeUso _llamarMozoCasoDeUso;
        private readonly ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso _obtenerDatosInvitadoBienvenidaAComandaCasoDeUso;
        private readonly ObtenerComandaActivaPorMesaCasoDeUso _obtenerComandaActivaPorMesaCasoDeUso;

        private readonly IComandaRepositorio _comandaRepositorio;

        private readonly ComandaMapper _mapper;



        public ComandaController(
            ListarComandaActivaCocinaCasoDeUso listarComandaActivasCasoDeUso,
            ModificarEstadoComandaCasoDeUso modificar,
            ConfirmarPedidoClienteAComandaCasoDeUso confirmarPedidoCasoDeUso,
            MarcarItemsEntregadosCasoDeUso marcarItemsEntregadosCasoDeUso,
            LlamarMozoComandaCasoDeUso llamarMozoCasoDeUso,
            ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso obtenerDatosInvitadoBienvenidaAComandaCasoDeUso,
            ObtenerComandaActivaPorMesaCasoDeUso obtenerComandaActivaPorMesaCasoDeUso,
            ComandaMapper mapper,
            IComandaRepositorio comandaRepositorio)
        {
            _listarComandasActivasCocinaCasoDeUso = listarComandaActivasCasoDeUso;
            _modificarEstadoComandaCasoDeUso = modificar;
            _confirmarPedidoCasoDeUso = confirmarPedidoCasoDeUso;
            _llamarMozoCasoDeUso = llamarMozoCasoDeUso;
            _marcarItemsEntregadosCasoDeUso = marcarItemsEntregadosCasoDeUso;
            _obtenerDatosInvitadoBienvenidaAComandaCasoDeUso = obtenerDatosInvitadoBienvenidaAComandaCasoDeUso;
            _obtenerComandaActivaPorMesaCasoDeUso = obtenerComandaActivaPorMesaCasoDeUso;
            _comandaRepositorio = comandaRepositorio;

            _mapper = mapper;


        }

        [HttpGet("activas")]
        [Authorize(Roles = "Cocina")]
        [ProducesResponseType(typeof(List<ComandaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ComandaResponseDto>>> ObtenerComandasActivas()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var comandas = await _listarComandasActivasCocinaCasoDeUso.Ejecutar(restauranteId);
            var comandasDto = _mapper.ComandaResponseDtoList(comandas);
            return Ok(comandasDto);
        }

        [HttpGet("mesa/{mesaId}/activa")]
        [Authorize(Roles = "Gerente, Mozo")]
        [ProducesResponseType(typeof(ComandaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ComandaResponseDto>> ObtenerComandaActivaPorMesa(int mesaId)
        {
            var comanda = await _obtenerComandaActivaPorMesaCasoDeUso.EjecutarAsync(mesaId);
            if (comanda == null)
            {
                return NotFound(new { mensaje = $"No se encontró una comanda activa para la mesa {mesaId}." });
            }
            var comandaDto = _mapper.ComandaResponseDto(comanda);
            return Ok(comandaDto);
        }
        [HttpPut("activas/{comandaId}/{estadoId}")]
        [Authorize(Roles = "Cocina")]
        [ProducesResponseType(typeof(List<ComandaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ComandaResponseDto>> ModificarEstadoDeComanda(int comandaId, int estadoId)
        {
            var comanda = await _modificarEstadoComandaCasoDeUso.EjecutarAsync(comandaId, estadoId);
            var comandaDto = _mapper.ComandaResponseDto(comanda);
            return Ok(comandaDto);
        }

        [HttpPut("{comandaId}/entregar-items")]
        [Authorize(Roles = "Mozo")]
        [ProducesResponseType(typeof(List<ComandaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ComandaResponseDto>> MarcarItemComandaEntregado(int comandaId, [FromBody] List<int> itemsEntregados)
        {
            var comanda = await _marcarItemsEntregadosCasoDeUso.EjecutarAsync(comandaId, itemsEntregados);
            var comandaDto = _mapper.ComandaResponseDto(comanda);
            return Ok(comandaDto);
        }

        [HttpGet("{comandaId}/comensal/bienvenida-invitado")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ComandaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerBienvenidaInvitado(int comandaId)
        {
            BienvenidaDatosInvitadoComanda datosDominio = await _obtenerDatosInvitadoBienvenidaAComandaCasoDeUso.EjecutarAsync(comandaId);
            return Ok(_mapper.aInvitadoBienvenidaComandaDto(datosDominio));
        }

        [HttpPost("{comandaId}/comensal/{restauranteId}/confirmar-pedido")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ComandaClienteEstadoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ComandaClienteEstadoResponseDto>> ConfirmarPedido(int comandaId, int restauranteId, [FromBody] ConfirmarPedidoClienteRequestDto request)
        {
            List<ArticuloComanda> articulosSolicitados = _mapper.ParaListaArticuloComandaDominio(request);
            Comanda comandaActualizada = await _confirmarPedidoCasoDeUso.EjecutarAsync(restauranteId, comandaId, articulosSolicitados);
            ComandaClienteEstadoResponseDto responseDto = _mapper.ParaEstadoClienteDto(comandaActualizada);
            return Ok(responseDto);
        }

        [HttpGet("{comandaId}/comensal/{restauranteId}/estado-pedido")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ComandaClienteEstadoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ComandaClienteEstadoResponseDto>> ObtenerEstadoComanda(int comandaId, int restauranteId)
        {
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);

            if (comanda == null || comanda.RestauranteId != restauranteId)
                return NotFound(new { mensaje = $"No se encontró la comanda con ID {comandaId}." });

            ComandaClienteEstadoResponseDto responseDto = _mapper.ParaEstadoClienteDto(comanda);

            return Ok(responseDto);
        }

        [HttpPost("{id}/llamar-mozo")]
        [ProducesResponseType(typeof(List<ComandaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LlamarMozo(int id)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            await _llamarMozoCasoDeUso.EjecutarAsync(restauranteId, id);
            return Ok(new { mensaje = "Notificación enviada al mozo exitosamente." });
        }




    }
}