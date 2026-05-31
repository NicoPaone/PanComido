using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Presentacion.DTOs.Pedidos;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Controllers
{
    [Route("pedido-proveedor")]
    [ApiController]
    public class PedidoProveedorController : ControllerBase
    {
        private readonly CrearPedidoCasoDeUso _crearPedidoCasoDeUso;
        private readonly EnviarPedidoProveedorCasoDeUso _confirmarPedidoCasoDeUso;
        private readonly PedidoMapper _pedidoMapper;
        private readonly GenerarSugerenciasRecepcionCasoDeUso _generarSugerenciasRecepcionCasoDeUso;
        private readonly LoteRecepcionMapper _recepcionPedidoMapper;
        private readonly RecibirPedidoProveedorCasoDeUso _recibirPedidoProveedorCasoDeUso;    

        public PedidoProveedorController(
            CrearPedidoCasoDeUso crearPedidoCasoDeUso,
            EnviarPedidoProveedorCasoDeUso confirmarPedidoCasoDeUso,
            PedidoMapper pedidoMapper,
            GenerarSugerenciasRecepcionCasoDeUso generarSugerenciasRecepcionCasoDeUso,
            LoteRecepcionMapper recepcionPedidoMapper,
            RecibirPedidoProveedorCasoDeUso recibirPedidoProveedorCasoDeUso)
        {
            _crearPedidoCasoDeUso = crearPedidoCasoDeUso;
            _confirmarPedidoCasoDeUso = confirmarPedidoCasoDeUso;
            _pedidoMapper = pedidoMapper;
            _generarSugerenciasRecepcionCasoDeUso = generarSugerenciasRecepcionCasoDeUso;
            _recepcionPedidoMapper = recepcionPedidoMapper;
            _recibirPedidoProveedorCasoDeUso = recibirPedidoProveedorCasoDeUso;
        }

        [HttpPost("{idProveedor}/crear-pedido")]
        public async Task<ActionResult<CrearPedidoResponseDto>> crear(
                    int idProveedor,
                    [FromBody] CrearPedidoRequestDto request)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var pedido = new DOM.Pedido
            {
                ProveedorId = idProveedor,
                ItemsInsumo = request.Items.Select(item => new DOM.PedidoInsumo
                {
                    InsumoId = item.InsumoId,
                    Cantidad = item.Cantidad,
                    PrecioCompra = item.PrecioCompra
                }).ToList()
            };

            var pedidoCreado = await _crearPedidoCasoDeUso.EjecutarAsync(pedido, restauranteId);
            var dto = _pedidoMapper.aDtoCrear(pedidoCreado);
            return Ok(dto);
        }


        [HttpPut("{pedidoId}/confirmar")]
        public async Task<ActionResult<EnviarPedidoResponseDto>> ConfirmarPedido(int pedidoId, [FromBody] ConfirmarPedidoRequestDto request)
        {
            var itemsInsumo = request.ListaInsumosPedido.Select(item => new DOM.PedidoInsumo
            {
                InsumoId = item.InsumoId,
                Cantidad = item.Cantidad,
                PrecioCompra = item.PrecioCompra
            }).ToList();

            var (pedido, linkWpp) = await _confirmarPedidoCasoDeUso.EjecutarAsync(pedidoId, itemsInsumo);

            var pedidoDto = _pedidoMapper.aDto(pedido);

            var dto = new EnviarPedidoResponseDto
            {
                PedidoConfirmado = pedidoDto,
                LinkWpp = linkWpp
            };

            return Ok(dto);
        }

        [HttpGet("{pedidoId}/previsualizar-confirmacion")]
        public async Task<ActionResult<string>> PrevisualizarConfirmacion(int pedidoId)
        {
            var sugerencias = await _generarSugerenciasRecepcionCasoDeUso.EjecutarAsync(pedidoId);
            var dtos = _recepcionPedidoMapper.aListaDto(sugerencias);
            return Ok(dtos);
        }

        [HttpPut("{pedidoId}/recibir")]
        public async Task<IActionResult> RecibirPedido(int pedidoId, [FromBody] RecibirPedidoRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var lotes = _recepcionPedidoMapper.aListaDominio(request.ItemsPedidoRecibido);
            await _recibirPedidoProveedorCasoDeUso.EjecutarAsync(pedidoId, lotes, restauranteId);
            return Ok(new { mensaje = "Pedido recibido y lotes creados correctamente." });
        }
    }
}