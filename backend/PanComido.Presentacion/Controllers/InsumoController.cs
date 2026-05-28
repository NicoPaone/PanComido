using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsumoController : ControllerBase
    {
        private readonly ListarInsumoCasoDeUso _listarInsumoCasoDeUso;
        private readonly InsumoMapper _mapper;

        public InsumoController(ListarInsumoCasoDeUso listarInsumoCasoDeUso, InsumoMapper mapper)
        {
            _listarInsumoCasoDeUso = listarInsumoCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<InsumoResponseDto>>> obtener(
            [FromQuery] string? categoria = null,
            [FromQuery] string? busqueda = null) {

            var restauranteId = ObtenerRestauranteId();

            // 1. Ejecutar Use Case → lista de entidades de Dominio con EstadoStock calculado
            var insumos = await _listarInsumoCasoDeUso.EjecutarAsync(
                restauranteId, categoria, busqueda);

            // 2. Dominio → DTOs
            var dtos = _mapper.aListaDto(insumos);
            return Ok(dtos);
        }

        // Helper temporal — luego lo pasaremos a id dinamico del restaurante
        private int ObtenerRestauranteId()
        {
            // TODO: reemplazar por: int.Parse(User.FindFirst("restauranteId")!.Value)
            return 1;
        }



    }
}
