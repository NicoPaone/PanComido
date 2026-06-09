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
    public class ProveedorController : ControllerBase
    {
        private readonly ListarProveedorCasoDeUso _listarProveedorCasoDeUso;
        private readonly ObtenerHistorialPedidosCasoDeUso _obtenerHistorialCasoDeUso;
        private readonly ListarInsumosDelProveedorCasoDeUso _listarInsumosDelProveedorCasoDeUso;
        private readonly ObtenerInsumosParaPedidoCasoDeUso _obtenerInsumosParaPedidoCasoDeUso;
        private readonly CrearProveedorCasoDeUso _crearProveedorCasoDeUso;
        private readonly ModificarProveedorCasoDeUso _modificarProveedorCasoDeUso;
        private readonly EliminarProveedorCasoDeuso _eliminarProveedorCasoDeUso;
        private readonly ObtenerProveedorCasoDeUso _obtenerProveedorCasoDeuso;

        private readonly ProveedorMapper _proveedorMapper;
        private readonly PedidoMapper _pedidoMapper;
        private readonly InsumoMapper _insumoMapper;
        private readonly InsumoConsugerenciaMapper _insumoConsugerenciaMapper;


        public ProveedorController(
            ListarProveedorCasoDeUso listarProveedorCasoDeUso,
            ObtenerHistorialPedidosCasoDeUso obtenerHistorialCasoDeUso,
            ListarInsumosDelProveedorCasoDeUso listarInsumosDelProveedorCasoDeUso,
            ObtenerInsumosParaPedidoCasoDeUso obtenerInsumosParaPedidoCasoDeUso,
            CrearProveedorCasoDeUso crearProveedorCasoDeUso,
            ModificarProveedorCasoDeUso modificarProveedorCasoDeUso,
            EliminarProveedorCasoDeuso eliminarProveedorCasoDeUso,
            ObtenerProveedorCasoDeUso obtenerProveedorCasoDeUso,
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
            _crearProveedorCasoDeUso = crearProveedorCasoDeUso;
            _modificarProveedorCasoDeUso = modificarProveedorCasoDeUso;
            _eliminarProveedorCasoDeUso = eliminarProveedorCasoDeUso;
            _obtenerProveedorCasoDeuso = obtenerProveedorCasoDeUso;
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

        [HttpPost("crear-proveedor")]
        public async Task<IActionResult> CrearProveedor([FromBody] ProveedorRequestDto proveedorRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var proveedorDominio = _proveedorMapper.aDominio(proveedorRequest);
            proveedorDominio.RestauranteId = restauranteId;
            var proveedorCreado = await _crearProveedorCasoDeUso.EjecutarAsync(proveedorDominio);
            return StatusCode(201, new
            {
                proveedorDto = _proveedorMapper.aDto(proveedorCreado),
                mensaje = "Proveedor creado correctamente."
            });
        }

        [HttpPatch("{idProveedor}/modificar-proveedor")]
        public async Task<IActionResult> ModificarProveedor(int idProveedor, [FromBody] ProveedorRequestDto proveedorRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var proveedorDominio = _proveedorMapper.aDominio(proveedorRequest);
            proveedorDominio.Id = idProveedor;
            proveedorDominio.RestauranteId = restauranteId;

           
            var proveedorModificado = await _modificarProveedorCasoDeUso.EjecutarAsync(proveedorDominio);
            return Ok(new
            {
                proveedorDto = _proveedorMapper.aDto(proveedorModificado),
                mensaje = "Proveedor modificado correctamente."
            });
        }

        [HttpDelete("{idProveedor}")]
        public async Task<IActionResult> EliminarProveedor(int idProveedor)
        {
            await _eliminarProveedorCasoDeUso.EjecutarAsync(idProveedor);
            return Ok(new { mensaje = "Proveedor eliminado correctamente." });
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

        [HttpGet("obtener-proveedor/{idProveedor}")]
        public async Task<ActionResult> ObtenerProveedorId(int idProveedor)
        {
            var proveedorEncontrado = await _obtenerProveedorCasoDeuso.EjecutarAsync(idProveedor);

            return Ok(_proveedorMapper.aDto(proveedorEncontrado));
        }
    }
}
