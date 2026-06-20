
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso;
using PanComido.Dominio.CasosDeUso.PlatoCasoDeUso;
using PanComido.Dominio.CasosDeUso.PlatoCasosDeUso;
using PanComido.Dominio.Constantes;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.Mesas;
using PanComido.Presentacion.DTOs.Plato;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("/plato")]
    [ApiController]
   [Authorize]

   public class PlatoController : ControllerBase
    {

        private readonly ObtenerDatosParaFormularioCrearPlato _obtenerDatosCasoDeUso;
        private readonly FormularioParaCrearPlatoMapper _mapper;

        private readonly CrearPlatoCasoDeUso _crearPlatoCasoDeUso;
        private readonly PlatoMapper _platoMapper;





        private readonly ModificarPlatoCasoDeUso _modificarPlatoCasoDeUso;
        private readonly ObtenerPlatoPorIdCasoDeUso _obtenerPlatoPorIdCasoDeUso;
        private readonly EliminarPlatoCasoDeUso _eliminarPlatoCasoDeUso;


        public PlatoController(ObtenerDatosParaFormularioCrearPlato obtenerDatosCasoDeUso, FormularioParaCrearPlatoMapper mapper, CrearPlatoCasoDeUso crearPlatoCasoDeUso, PlatoMapper platoMapper, ModificarPlatoCasoDeUso modificarPlatoCasoDeUso, ObtenerPlatoPorIdCasoDeUso obtenerPlatoPorIdCasoDeUso, EliminarPlatoCasoDeUso eliminarPlatoCasoDeUso)
        {
            _obtenerDatosCasoDeUso = obtenerDatosCasoDeUso;
            this._mapper = mapper;
            _crearPlatoCasoDeUso = crearPlatoCasoDeUso;
            _platoMapper = platoMapper;
            _modificarPlatoCasoDeUso = modificarPlatoCasoDeUso;
            _obtenerPlatoPorIdCasoDeUso = obtenerPlatoPorIdCasoDeUso;
            _eliminarPlatoCasoDeUso = eliminarPlatoCasoDeUso;
        }

        
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPlatoPorId(int id)
        {
            int restauranteId = 1; 
            
            if (HttpContext.Items.TryGetValue("restauranteId", out var idSesion))
            {
                restauranteId = (int)idSesion;
            }
            else if (int.TryParse(HttpContext.Request.Query["restauranteId"], out int queryRestauranteId))
            {
                restauranteId = queryRestauranteId;
            }

            var platoDominio = await _obtenerPlatoPorIdCasoDeUso.EjecutarAsync(id, restauranteId);
            
            if (platoDominio == null)
            {
                return NotFound(new { mensaje = "El plato no existe." });
            }

            return Ok(_platoMapper.aDto(platoDominio));
        }

        [HttpGet("formulario-plato")]
        [ProducesResponseType(typeof(List<DatosFormularioCrearPlatoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DatosFormularioCrearPlatoResponseDto>> ObtenerDatosFormulario()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var datosDominio = await _obtenerDatosCasoDeUso.Ejecutar(restauranteId);

            return Ok(_mapper.aDto(datosDominio));
        }
        
      [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromForm] CrearPlatoDto request, IFormFile? imagen)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }
         
         int restauranteId = HttpContext.ObtenerRestauranteId();
         var platoDominio = _platoMapper.aDominio(request);

         Stream? stream = imagen?.OpenReadStream();
         string? nombreArchivo = imagen?.FileName;
         
         await _crearPlatoCasoDeUso.EjecutarAsync(restauranteId, platoDominio,
               RutasCloudinary.MenuPlatos,
               stream,
               nombreArchivo);
               
         return StatusCode(201, new { mensaje = "Plato creado correctamente." });
      }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Modificar(int id, [FromBody] ModificarPlatoDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int restauranteId = HttpContext.ObtenerRestauranteId();

            var platoDominio = _platoMapper.ModificarADominio(id, request);

            await _modificarPlatoCasoDeUso.EjecutarAsync(restauranteId, platoDominio);

            return Ok(new { mensaje = "Plato modificado correctamente." });
        }




        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await _eliminarPlatoCasoDeUso.EjecutarAsync(id, restauranteId);
            return Ok(new { mensaje = "Plato eliminado correctamente." });
        }
    }

}