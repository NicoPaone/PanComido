using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : Controller
    {
        private readonly ListarProveedorCasoDeUso _listarProveedorCasoDeUso;
        private readonly ObtenerHistorialPedidosCasoDeUso _obtenerHistorialCasoDeUso;

        private readonly ProveedorMapper _proveedorMapper;
        private readonly PedidoMapper _pedidoMapper;


        public ProveedorController(
            ListarProveedorCasoDeUso listarProveedorCasoDeUso,
            ObtenerHistorialPedidosCasoDeUso obtenerHistorialCasoDeUso,
            ProveedorMapper proveedorMapper,
            PedidoMapper pedidoMapper
            )
        {
            _listarProveedorCasoDeUso = listarProveedorCasoDeUso;
            _obtenerHistorialCasoDeUso = obtenerHistorialCasoDeUso;
            _proveedorMapper = proveedorMapper;
            _pedidoMapper = pedidoMapper;
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

        private int ObtenerRestauranteId()
        {
            return 1;
        }
    }
}
