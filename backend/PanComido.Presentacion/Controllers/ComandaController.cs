using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.Cliente;
using PanComido.Presentacion.DTOs.Pedidos;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("comanda")]
    [ApiController]
    public class ComandaController : ControllerBase
    {
        private readonly ListarComandaActivaCocinaCasoDeUso _listarComandasActivasCocinaCasoDeUso;
        private readonly ModificarEstadoComandaCasoDeUso _modificarEstadoComandaCasoDeUso;
        private readonly MarcarItemEntregadoCasoDeUso _marcarItemEntregadoCasoDeUso;
        private readonly ConfirmarPedidoClienteAComandaCasoDeUso _confirmarPedidoCasoDeUso;
        private readonly ComandaMapper _mapper;

        private readonly IComandaRepositorio _comandaRepositorio;


        public ComandaController(
            ListarComandaActivaCocinaCasoDeUso listarComandaActivasCasoDeUso,
            ModificarEstadoComandaCasoDeUso modificar,
            MarcarItemEntregadoCasoDeUso marcarItemEntregadoCasoDeUso,
            ConfirmarPedidoClienteAComandaCasoDeUso confirmarPedidoCasoDeUso,
            ComandaMapper mapper,
            IComandaRepositorio comandaRepositorio)
        {
            _listarComandasActivasCocinaCasoDeUso = listarComandaActivasCasoDeUso;
            _modificarEstadoComandaCasoDeUso = modificar;
            _marcarItemEntregadoCasoDeUso = marcarItemEntregadoCasoDeUso;
            _confirmarPedidoCasoDeUso = confirmarPedidoCasoDeUso;
            _mapper = mapper;
            _comandaRepositorio = comandaRepositorio;

        }

        [HttpGet("activas")]
        public async Task<ActionResult<List<ComandaResponseDto>>> ObtenerComandasActivas()
        {
            int restauranteId = 1;
            var comandas = await _listarComandasActivasCocinaCasoDeUso.Ejecutar(restauranteId);
            var comandasDto = _mapper.ComandaResponseDtoList(comandas);
            return Ok(comandasDto);

        }
        [HttpPut("activas/{mesaId}/{estadoId}")]
        public async Task<ActionResult<ComandaResponseDto>> ModificarEstadoDeComanda(int mesaId, int estadoId)
        {
            var comanda = await _modificarEstadoComandaCasoDeUso.EjecutarAsync(mesaId, estadoId);
            var comandaDto = _mapper.ComandaResponseDto(comanda);
            return Ok(comandaDto);
        }

        [HttpPut("{comandaId}/item/{articuloComandaId}/entregar")]
        public async Task<ActionResult<ComandaResponseDto>> MarcarItemComandaEntregado(int comandaId, int articuloComandaId)
        {
            var comanda = await _marcarItemEntregadoCasoDeUso.EjecutarAsync(comandaId, articuloComandaId);
            var comandaDto = _mapper.ComandaResponseDto(comanda);
            return Ok(comandaDto);
        }

        [HttpPost("{comandaId}/cliente/confirmar-pedido")]
        public async Task<ActionResult<ComandaClienteEstadoResponseDto>> ConfirmarPedido(int comandaId, [FromBody] ConfirmarPedidoClienteRequestDto request)
        {
            try
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

                List<ArticuloComanda> articulosSolicitados = _mapper.ParaListaArticuloComandaDominio(request);

                Comanda comandaActualizada = await _confirmarPedidoCasoDeUso.EjecutarAsync(restauranteId, comandaId, articulosSolicitados);

                ComandaClienteEstadoResponseDto responseDto = _mapper.ParaEstadoClienteDto(comandaActualizada);

                return Ok(responseDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al procesar el pedido.", detalle = ex.Message });
            }
        }

        [HttpGet("{comandaId}/cliente/estado-pedido")]
        public async Task<ActionResult<ComandaClienteEstadoResponseDto>> ObtenerEstadoComanda(int comandaId)
        {
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);

            if (comanda == null)
                return NotFound(new { mensaje = $"No se encontró la comanda con ID {comandaId}." });

            ComandaClienteEstadoResponseDto responseDto = _mapper.ParaEstadoClienteDto(comanda);

            return Ok(responseDto);
        }
    }
}
