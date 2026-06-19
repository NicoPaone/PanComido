
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso;
using PanComido.Dominio.CasosDeUso.PlatoCasoDeUso;
using PanComido.Dominio.CasosDeUso.PlatoCasosDeUso;
using PanComido.Dominio.Constantes;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.Plato;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("/plato")]
    [ApiController]
   [Authorize]

   public class PlatoController : ControllerBase
    {

        //dependencias del GET (formulario para crear plato)
        private readonly ObtenerDatosParaFormularioCrearPlato _obtenerDatosCasoDeUso;
        private readonly FormularioParaCrearPlatoMapper _mapper;

        // dependencias del POST (crear plato)
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
        public async Task<IActionResult> ObtenerPlatoPorId(int id)
        {
            // Si viene autenticado, obtener del context; si no, del query param o valor por defecto
            int restauranteId = 1; // valor por defecto
            try
            {
                restauranteId = HttpContext.ObtenerRestauranteId();
            }
            catch
            {
                // Si no hay token válido, intentar obtener del query param
                if (int.TryParse(HttpContext.Request.Query["restauranteId"], out int queryRestauranteId))
                {
                    restauranteId = queryRestauranteId;
                }
            }

            var platoDominio = await _obtenerPlatoPorIdCasoDeUso.EjecutarAsync(id, restauranteId);
            
            if (platoDominio == null)
            {
                return NotFound(new { mensaje = "El plato no existe." });
            }

            return Ok(_platoMapper.aDto(platoDominio));
        }

        [HttpGet("formulario-plato")]
        public async Task<ActionResult<DatosFormularioCrearPlatoResponseDto>> ObtenerDatosFormulario()
        {
            // Extraemos el ID del filtro de contexto, sin hardcodear nada
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var datosDominio = await _obtenerDatosCasoDeUso.Ejecutar(restauranteId);

            return Ok(_mapper.aDto(datosDominio));
        }
        
      [HttpPost]
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
        public async Task<IActionResult> Eliminar(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await _eliminarPlatoCasoDeUso.EjecutarAsync(id, restauranteId);
            return Ok(new { mensaje = "Plato eliminado correctamente." });
        }
    }

}