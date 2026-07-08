using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Constantes;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("insumo")]
    [ApiController]
   [Authorize(Roles = "Gerente")]

   public class InsumoController : ControllerBase
    {
        private readonly ListarInsumoCasoDeUso _listarInsumoCasoDeUso;
        private readonly CrearInsumoCasoDeUso _crearInsumoCasoDeUso;
        private readonly ModificarInsumoCasoDeUso _modificarInsumoCasoDeUso;
        private readonly ObtenerInsumoPorIdCasoDeUso _obtenerInsumoPorIdCasoDeUso;
        private readonly EliminarInsumoCasoDeUso _eliminarInsumoCasoDeUso;
        private readonly InsumoMapper _mapper;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly LoteMapper _loteMapper;

        public InsumoController(
            ListarInsumoCasoDeUso listarInsumoCasoDeUso,
            CrearInsumoCasoDeUso crearInsumoCasoDeUso,
            ModificarInsumoCasoDeUso modificarInsumoCasoDeUso,
            ObtenerInsumoPorIdCasoDeUso obtenerInsumoPorIdCasoDeUso,
            EliminarInsumoCasoDeUso eliminarInsumoCasoDeUso,
            InsumoMapper mapper,
            ILoteRepositorio loteRepositorio,
            LoteMapper loteMapper)
        {
            _listarInsumoCasoDeUso = listarInsumoCasoDeUso;
            _crearInsumoCasoDeUso = crearInsumoCasoDeUso;
            _modificarInsumoCasoDeUso = modificarInsumoCasoDeUso;
            _obtenerInsumoPorIdCasoDeUso = obtenerInsumoPorIdCasoDeUso;
            _eliminarInsumoCasoDeUso = eliminarInsumoCasoDeUso;
            _mapper = mapper;
            _loteRepositorio = loteRepositorio;
            _loteMapper = loteMapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<InsumoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InsumoResponseDto>>> obtener() {

            var restauranteId = HttpContext.ObtenerRestauranteId();

            var insumos = await _listarInsumoCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _mapper.aListaDto(insumos);
            return Ok(dtos);
        }

        [HttpGet("lotes")]
        public async Task<IActionResult> ObtenerLotes()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var lotes = await _loteRepositorio.ObtenerLotesPorRestauranteAsync(restauranteId);
            return Ok(_loteMapper.aListaDto(lotes));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] CrearInsumoRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           
            int restauranteId = HttpContext.ObtenerRestauranteId();

            Insumo insumoDominio = _mapper.aDominio(request);

            Stream? stream = request.Imagen?.OpenReadStream();
            string? nombreArchivo = request.Imagen?.FileName;

            Insumo insumoCreado = await _crearInsumoCasoDeUso.EjecutarAsync(
                restauranteId,
                insumoDominio,
                request.CantidadInicial,
                request.BodegaId,
                request.FechaVencimiento,
                stream,
                nombreArchivo,
                // TODO: HACER UNO ESPECIFICO PARA INSUMOS
                RutasCloudinary.MenuPlatos
            );

            return StatusCode(201, new {
                insumo = _mapper.aDto(insumoCreado),
                mensaje = "Insumo creado correctamente." 
            });
            
        }

        [HttpPut("{insumoId}")]
        public async Task<IActionResult> ModificarInsumo(int insumoId, [FromForm] ModificarInsumoRequestDto insumoRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int restauranteId = HttpContext.ObtenerRestauranteId();

            Insumo insumoDominio = _mapper.ModificarADominio(insumoId, insumoRequest);

            Stream? stream = insumoRequest.Imagen?.OpenReadStream();
            string? nombreArchivo = insumoRequest.Imagen?.FileName;

            Insumo insumoModificado = await _modificarInsumoCasoDeUso.EjecutarAsync(
                restauranteId,
                insumoDominio,
                stream,
                nombreArchivo,
                // TODO: HACER UNO ESPECIFICO PARA INSUMOS (mismo TODO que ya tenés en Crear)
                RutasCloudinary.MenuPlatos
            );

            return Ok(new
            {
                insumo = _mapper.aDto(insumoModificado),
                mensaje = "Insumo modificado correctamente."
            });
        }

        [HttpGet("{insumoId}")]
        public async Task<IActionResult> ObtenerInsumoPorId(int insumoId)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            Insumo insumoDominio = await _obtenerInsumoPorIdCasoDeUso.EjecutarAsync(insumoId, restauranteId);

            return Ok(_mapper.aDetalleDto(insumoDominio));
        }

        [HttpDelete("{insumoId}")]
        public async Task<IActionResult> Eliminar(int insumoId)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await _eliminarInsumoCasoDeUso.EjecutarAsync(insumoId, restauranteId);
            return Ok(new { mensaje = "Insumo eliminado correctamente." });
        }
    }
}
