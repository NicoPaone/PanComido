using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.MiseAndPlace;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;
using PanComido.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("/miseandplace")]
    [ApiController]
    [Authorize]
    public class MiseAndPlaceController : ControllerBase
    {
        private readonly ObtenerIngredientesParaCrearMiseAndPlace _obtenerIngredientesCasoDeUso;
        private readonly CrearMiseAndPlaceCasoDeUso _crearMiseAndPlaceCasoDeUso;
        private readonly ObtenerTodosLosMiseAndPlaceCasoDeUso _obtenerTodosCasoDeUso;
        private readonly ObtenerMiseAndPlacePorIdCasoDeUso _obtenerPorIdCasoDeUso;
        private readonly MiseAndPlaceMapper _mapper;

        public MiseAndPlaceController(
            ObtenerIngredientesParaCrearMiseAndPlace obtenerIngredientesCasoDeUso, 
            CrearMiseAndPlaceCasoDeUso crearMiseAndPlaceCasoDeUso,
            ObtenerTodosLosMiseAndPlaceCasoDeUso obtenerTodosCasoDeUso,
            ObtenerMiseAndPlacePorIdCasoDeUso obtenerPorIdCasoDeUso,
            MiseAndPlaceMapper mapper)
        {
            _obtenerIngredientesCasoDeUso = obtenerIngredientesCasoDeUso;
            _crearMiseAndPlaceCasoDeUso = crearMiseAndPlaceCasoDeUso;
            _obtenerTodosCasoDeUso = obtenerTodosCasoDeUso;
            _obtenerPorIdCasoDeUso = obtenerPorIdCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet("obtener-ingredientes")]
        [ProducesResponseType(typeof(DatosFormularioMiseAndPlaceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerIngredientes()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var (ingredientesDominio, categoriasDominio, unidadesDominio, bodegasDominio) = await _obtenerIngredientesCasoDeUso.EjecutarAsync(restauranteId);

            var respuesta = new DatosFormularioMiseAndPlaceDto
            {
                Ingredientes = _mapper.aDtoList(ingredientesDominio),
                Categorias = categoriasDominio.ConvertAll(c => _mapper.aDtoCategoria(c)),
                UnidadesMedida = unidadesDominio.ConvertAll(u => _mapper.aDtoUnidad(u)),
                Bodegas = bodegasDominio.ConvertAll(b => _mapper.aDtoBodega(b))
            };

            return Ok(respuesta);
        }
        [HttpPost("crear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearMiseAndPlace([FromBody] CrearMiseAndPlaceDto dto)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var nuevoMiseAndPlace = new NuevoMiseAndPlace
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Cantidad = dto.Cantidad,
                FechaVencimiento = dto.FechaVencimiento,
                UnidadMedidaId = dto.UnidadMedidaId,
                CategoriaId = dto.CategoriaId,
                BodegaId = dto.BodegaId,
                RestauranteId = restauranteId,
                Ingredientes = dto.Ingredientes.ConvertAll(i => new IngredienteDeMiseAndPlace
                {
                    IngredienteId = i.IngredienteId,
                    Cantidad = i.Cantidad
                })
            };
         int id = await _crearMiseAndPlaceCasoDeUso.EjecutarAsync(nuevoMiseAndPlace);
         var dominio = await _obtenerPorIdCasoDeUso.EjecutarAsync(restauranteId, id);

         return CreatedAtAction(nameof(ObtenerPorId), new { id }, _mapper.aDtoListado(dominio));
      }

      [HttpGet("listar")]
        [ProducesResponseType(typeof(List<MiseAndPlaceListadoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarMiseAndPlace()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var listadoDominio = await _obtenerTodosCasoDeUso.EjecutarAsync(restauranteId);

            var respuesta = listadoDominio.ConvertAll(m => _mapper.aDtoListado(m));

            return Ok(respuesta);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MiseAndPlaceListadoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var dominio = await _obtenerPorIdCasoDeUso.EjecutarAsync(restauranteId, id);

            if (dominio == null)
            {
                return NotFound(new ErrorResponseDto { Error = "Mise and Place no encontrado." });
            }

            var respuesta = _mapper.aDtoListado(dominio);

            return Ok(respuesta);
        }
    }
}
