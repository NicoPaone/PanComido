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
        private readonly ProveedorMapper _mapper;

        public ProveedorController(ListarProveedorCasoDeUso listarProveedorCasoDeUso, ProveedorMapper mapper)
        {
            _listarProveedorCasoDeUso = listarProveedorCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProveedorResponseDto>>> obtener()
        {
            var restauranteId = ObtenerRestauranteId();

            var proveedores = await _listarProveedorCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _mapper.aListaDto(proveedores);
            return Ok(dtos);
        }

        private int ObtenerRestauranteId()
        {
            return 1;
        }
    }
}
