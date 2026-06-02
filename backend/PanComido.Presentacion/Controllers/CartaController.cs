using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Presentacion.DTOs.Articulos;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("restaurante/{restauranteId}/carta")]
    [ApiController]
    public class CartaController : ControllerBase
    {
        private readonly ObtenerCartaCasoDeUso _obtenerCartaCasoDeUso;
        private readonly CartaMapper _mapper;

        public CartaController(ObtenerCartaCasoDeUso obtenerCartaCasoDeUso, CartaMapper mapper)
        {
            _obtenerCartaCasoDeUso = obtenerCartaCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCartaDisponible(int restauranteId)
        {
            var articulosDisponibles = await _obtenerCartaCasoDeUso.EjecutarAsync(restauranteId);

            var respuestaJson = _mapper.ParaDtoList(articulosDisponibles);

            return Ok(respuestaJson);
        }
    }
}
