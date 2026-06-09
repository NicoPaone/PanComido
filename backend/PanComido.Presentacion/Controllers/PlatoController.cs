
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso;
using PanComido.Dominio.CasosDeUso.PlatoCasosDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.Plato;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("/plato")]
    [ApiController]
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


        public PlatoController(ObtenerDatosParaFormularioCrearPlato obtenerDatosCasoDeUso, FormularioParaCrearPlatoMapper mapper, CrearPlatoCasoDeUso crearPlatoCasoDeUso, PlatoMapper platoMapper, ModificarPlatoCasoDeUso modificarPlatoCasoDeUso, ObtenerPlatoPorIdCasoDeUso obtenerPlatoPorIdCasoDeUso)
        {
            _obtenerDatosCasoDeUso = obtenerDatosCasoDeUso;
            this._mapper = mapper;
            _crearPlatoCasoDeUso = crearPlatoCasoDeUso;
            _platoMapper = platoMapper;
            _modificarPlatoCasoDeUso = modificarPlatoCasoDeUso;
            _obtenerPlatoPorIdCasoDeUso = obtenerPlatoPorIdCasoDeUso;

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPlatoPorId(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
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
        public async Task<IActionResult> Crear([FromBody] CrearPlatoDto request)
        {
            // Angular manda el JSON. Si le faltó un campo obligatorio, esto ataja el error
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Cero Try-Catch en nuevos endpoints según reglas, pero este es el viejo, lo dejo intacto
            try
            {
                // 1. Obtenemos el ID del restaurante del token/contexto de forma segura
                int restauranteId = HttpContext.ObtenerRestauranteId();

                // 2. Usamos el Mapper para traducir la caja de Angular (DTO) a nuestra Entidad de Dominio
                var platoDominio = _platoMapper.aDominio(request);

                // 3. Llamamos al Caso de Uso para aplicar las reglas de negocio y guardar
                await _crearPlatoCasoDeUso.EjecutarAsync(restauranteId, platoDominio);

                // 4. Devolvemos código HTTP 201 Created con un mensaje de éxito
                return StatusCode(201, new { mensaje = "Plato creado correctamente." });
            }
            catch (ArgumentException ex)
            {
                // Si el Caso de Uso tira un error (ej: "El precio debe ser mayor a 0"), devolvemos 400 Bad Request
                return BadRequest(new { error = ex.Message });
            }
            catch (System.Exception ex)
            {
                // Si explota la base de datos o hay un error grave, devolvemos 500 Internal Server Error
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
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




    }

}