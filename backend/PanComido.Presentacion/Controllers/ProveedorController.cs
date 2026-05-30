using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.DTOs.Pedidos;
using PanComido.Presentacion.DTOs.Proveedores;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("proveedor")]
    [ApiController]
    public class ProveedorController : Controller
    {
        private readonly ListarProveedorCasoDeUso _listarProveedorCasoDeUso;
        private readonly ObtenerHistorialPedidosCasoDeUso _obtenerHistorialCasoDeUso;
        private readonly ListarInsumosDelProveedorCasoDeUso _listarInsumosDelProveedorCasoDeUso;
        private readonly ObtenerInsumosParaPedidoCasoDeUso _obtenerInsumosParaPedidoCasoDeUso;

        private readonly ProveedorMapper _proveedorMapper;
        private readonly PedidoMapper _pedidoMapper;
        private readonly InsumoMapper _insumoMapper;
        private readonly InsumoConsugerenciaMapper _insumoConsugerenciaMapper;


        public ProveedorController(
            ListarProveedorCasoDeUso listarProveedorCasoDeUso,
            ObtenerHistorialPedidosCasoDeUso obtenerHistorialCasoDeUso,
            ListarInsumosDelProveedorCasoDeUso listarInsumosDelProveedorCasoDeUso,
            ObtenerInsumosParaPedidoCasoDeUso obtenerInsumosParaPedidoCasoDeUso,
            ProveedorMapper proveedorMapper,
            PedidoMapper pedidoMapper,
            InsumoMapper insumoMapper,
            InsumoConsugerenciaMapper insumoConsugerenciaMapper
            )
        {
            _listarProveedorCasoDeUso = listarProveedorCasoDeUso;
            _obtenerHistorialCasoDeUso = obtenerHistorialCasoDeUso;
            _listarInsumosDelProveedorCasoDeUso = listarInsumosDelProveedorCasoDeUso;
            _obtenerInsumosParaPedidoCasoDeUso = obtenerInsumosParaPedidoCasoDeUso;
            _proveedorMapper = proveedorMapper;
            _pedidoMapper = pedidoMapper;
            _insumoMapper = insumoMapper;
            _insumoConsugerenciaMapper = insumoConsugerenciaMapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProveedorResponseDto>>> obtener()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

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
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var insumos = await _listarInsumosDelProveedorCasoDeUso.EjecutarAsync(idProveedor, restauranteId);

            var dtos = _insumoMapper.aListaDto(insumos);
            return Ok(dtos);
        }

        [HttpGet("{idProveedor}/insumos-a-reponer")]
        public async Task<ActionResult<List<InsumoParaReponerResponseDto>>> obtenerInsumosAReponer(int idProveedor)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId(); 
            var insumosSugeridos = await _obtenerInsumosParaPedidoCasoDeUso.EjecutarAsync(idProveedor, restauranteId);

            var dtos = _insumoConsugerenciaMapper.aListaDto(insumosSugeridos);
            return Ok(dtos);
        }
    }
}
