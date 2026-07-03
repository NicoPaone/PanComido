using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.EncuestaCasosDeUso;
using PanComido.Presentacion.DTOs.Encuesta;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("encuesta")]
    [ApiController]
    public class EncuestasController : ControllerBase
    {
        private readonly CrearEncuestaSatisfaccionCasoDeUso _crearEncuestaCasoDeUso;
        private readonly EncuestaMapper _mapper;
        public EncuestasController(
            CrearEncuestaSatisfaccionCasoDeUso crearEncuestaCasoDeUso,
            EncuestaMapper mapper)
        {
            _crearEncuestaCasoDeUso = crearEncuestaCasoDeUso;
            _mapper = mapper;
        }
        [HttpPost]
        [ProducesResponseType(typeof(EncuestaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)] 
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EncuestaResponseDto>> EnviarEncuesta([FromBody] EncuestaRequestDto request)
        {
            var encuestaDominio = _mapper.RequestDtoADominio(request);
            var googleLink = await _crearEncuestaCasoDeUso.EjecutarAsync(encuestaDominio);
            return Ok(_mapper.GoogleLinkAResponseDto(googleLink));
        }
    }
}
