using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Presentacion.DTOs.Insumos;

namespace PanComido.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : Controller
    {
        private readonly ListarProveedorCasoDeUso _listarProveedorCasoDeUso;
        private readonly ObtenerHistorialPedidosCasoDeUso _obtenerHistorialCasoDeUso;
        private readonly ListarInsumosDelProveedorCasoDeUso _listarInsumosDelProveedorCasoDeUso;
        private readonly CrearPedidoCasoDeUso _crearPedidoCasoDeUso;

        private readonly IProveedorRepositorio _proveedorRepositorio;

        private readonly ProveedorMapper _proveedorMapper;
        private readonly PedidoMapper _pedidoMapper;
        private readonly InsumoMapper _insumoMapper;


        public ProveedorController(
            ListarProveedorCasoDeUso listarProveedorCasoDeUso,
            ObtenerHistorialPedidosCasoDeUso obtenerHistorialCasoDeUso,
            ListarInsumosDelProveedorCasoDeUso listarInsumosDelProveedorCasoDeUso,
            CrearPedidoCasoDeUso crearPedidoCasoDeUso,
            IProveedorRepositorio proveedorRepositorio,
            ProveedorMapper proveedorMapper,
            PedidoMapper pedidoMapper,
            InsumoMapper insumoMapper
            )
        {
            _listarProveedorCasoDeUso = listarProveedorCasoDeUso;
            _obtenerHistorialCasoDeUso = obtenerHistorialCasoDeUso;
            _listarInsumosDelProveedorCasoDeUso = listarInsumosDelProveedorCasoDeUso;
            _crearPedidoCasoDeUso = crearPedidoCasoDeUso;
            _proveedorRepositorio = proveedorRepositorio;
            _proveedorMapper = proveedorMapper;
            _pedidoMapper = pedidoMapper;
            _insumoMapper = insumoMapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProveedorResponseDto>>> obtener()
        {
            var restauranteId = ObtenerRestauranteId();

            var proveedores = await _listarProveedorCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _proveedorMapper.aListaDto(proveedores);
            return Ok(dtos);
        }

        [HttpGet("{idProveedor}/historial-pedidos")]
        public async Task<ActionResult<List<PedidoResponseDto>>> obtenerHistorialPedidos(int idProveedor)
        {
            var pedidos = await _obtenerHistorialCasoDeUso.EjecutarAsync(idProveedor);
            if (pedidos == null) return NotFound();

            var dtos = _pedidoMapper.aListaDto(pedidos);
            return Ok(dtos);
        }

        [HttpGet("{idProveedor}/insumos")]
        public async Task<ActionResult<List<InsumoResponseDto>>> obtenerInsumos(int idProveedor)
        {
            var restauranteId = ObtenerRestauranteId();
            var insumos = await _listarInsumosDelProveedorCasoDeUso.EjecutarAsync(idProveedor, restauranteId);

            var dtos = _insumoMapper.aListaDto(insumos);
            return Ok(dtos);
        }

        [HttpPost("{idProveedor}/crearPedido")]
        public async Task<ActionResult<CrearPedidoResponseDto>> crear(
                            int idProveedor,
                            [FromBody] CrearPedidoRequestDto request)
        {
            var restauranteId = ObtenerRestauranteId();

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

        private int ObtenerRestauranteId()
        {
            return 1;
        }
    }
}
