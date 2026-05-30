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
        private readonly ConfirmarPedidoCasoDeUso _confirmarPedidoCasoDeUso;
        private readonly IProveedorRepositorio _proveedorRepositorio;
        private readonly PedidoMapper _pedidoMapper;

        public PedidoProveedorController(
            CrearPedidoCasoDeUso crearPedidoCasoDeUso,
            ConfirmarPedidoCasoDeUso confirmarPedidoCasoDeUso,
            IProveedorRepositorio proveedorRepositorio,
            PedidoMapper pedidoMapper)
        {
            _crearPedidoCasoDeUso = crearPedidoCasoDeUso;
            _confirmarPedidoCasoDeUso = confirmarPedidoCasoDeUso;
            _proveedorRepositorio = proveedorRepositorio;
            _pedidoMapper = pedidoMapper;
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

            var proveedor = await _proveedorRepositorio.ObtenerProveedorPorIdAsync(idProveedor);

            var dto = _pedidoMapper.aDtoCrear(pedidoCreado, proveedor);
            return Ok(dto);
        }


        [HttpPut("{pedidoId}/confirmar")]
        public async Task<ActionResult<ConfirmarPedidoResponseDto>> ConfirmarPedido(int pedidoId, [FromBody] ConfirmarPedidoRequestDto request)
        {
            var itemsInsumo = request.ListaInsumosPedido.Select(item => new DOM.PedidoInsumo
            {
                InsumoId = item.InsumoId,
                Cantidad = item.Cantidad,
                PrecioCompra = item.PrecioCompra
            }).ToList();

            var (pedido, linkWpp) = await _confirmarPedidoCasoDeUso.EjecutarAsync(pedidoId, itemsInsumo);

            var pedidoDto = _pedidoMapper.aDto(pedido);

            var dto = new ConfirmarPedidoResponseDto
            {
                PedidoConfirmado = pedidoDto,
                LinkWpp = linkWpp
            };

            return Ok(dto);
        }
    }
}