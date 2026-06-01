
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso;

using PanComido.Presentacion.DTOs;

using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("/plato")]
    [ApiController]
    public class PlatoController : ControllerBase
    {
        private readonly ObtenerDatosParaFormularioCrearPlato _obtenerDatosCasoDeUso;
        private readonly FormularioParaCrearPlatoMapper   _mapper;

       
       public PlatoController(ObtenerDatosParaFormularioCrearPlato obtenerDatosCasoDeUso, FormularioParaCrearPlatoMapper mapper)
        {
            _obtenerDatosCasoDeUso = obtenerDatosCasoDeUso;
            this._mapper = mapper;
        }

        [HttpGet("formulario-datos")]
        public async Task<ActionResult<DatosFormularioCrearPlatoResponseDto>> ObtenerDatosFormulario()
        {
            // Extraemos el ID del filtro de contexto, sin hardcodear nada
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var datosDominio = await _obtenerDatosCasoDeUso.Ejecutar(restauranteId);

            return Ok(_mapper.aDto(datosDominio));
        }
    }
}