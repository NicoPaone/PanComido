using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.Comanda;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ComandaController : ControllerBase
    {

        private readonly ListarComandaActivaCocinaCasoDeUso listarComandasActivasCasoDeUso;
        private readonly ComandaMapper _mapper;

        public ComandaController(ListarComandaActivaCocinaCasoDeUso listarComandaActivasCasoDeUso, ComandaMapper mapper)
        {
            listarComandasActivasCasoDeUso = listarComandaActivasCasoDeUso;
            _mapper = mapper;
        }


        [HttpGet("activas")]
        public async Task<ActionResult<List<ComandaResponseDto>>> ObtenerComandasActivas()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId(); 

            var comandas = await listarComandasActivasCasoDeUso.Ejecutar(restauranteId);

            var comandasDto = _mapper.ComandaResponseDtoList(comandas);

            return Ok(comandasDto);

        }

    }

    
}
