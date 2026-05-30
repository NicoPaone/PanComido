using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
      private readonly ModificarEstadoComandaCasoDeUso modificarEstadoComandaCasoDeUso;
      private readonly ComandaMapper _mapper;
      public ComandaController(ListarComandaActivaCocinaCasoDeUso listarComandaActivasCasoDeUso, ModificarEstadoComandaCasoDeUso modificar, ComandaMapper mapper)
      {
         listarComandasActivasCasoDeUso = listarComandaActivasCasoDeUso;
         modificarEstadoComandaCasoDeUso = modificar;
         _mapper = mapper;
      
      }
      [HttpGet("activas")]
      public async Task<ActionResult<List<ComandaResponseDto>>> ObtenerComandasActivas()
      {
         int restauranteId = 1;
         var comandas = await listarComandasActivasCasoDeUso.Ejecutar(restauranteId);
         var comandasDto = _mapper.ComandaResponseDtoList(comandas);
         return Ok(comandasDto);

      }
      [HttpPut("activas/{mesaId}/{estadoId}")]
      public async Task<ActionResult<ComandaResponseDto>> ModificarEstadoDeComanda(int mesaId, int estadoId)
      {
         var comanda = await modificarEstadoComandaCasoDeUso.EjecutarAsync(mesaId, estadoId);
         var comandaDto = _mapper.ComandaResponseDto(comanda);
         return Ok(comandaDto);

      }


        [HttpGet("activas")]
        public async Task<ActionResult<List<ComandaResponseDto>>> ObtenerComandasActivas()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId(); 


      

   }

    
}
