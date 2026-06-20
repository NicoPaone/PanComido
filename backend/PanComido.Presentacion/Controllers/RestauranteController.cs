using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Presentacion.DTOs.Restaurante;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
   [Route("restaurante")]
   [ApiController]
   public class RestauranteController : ControllerBase
   {
      private readonly ObtenerDatosDelLocalCasoDeUso _datosLocal;
      private readonly RestauranteMapper _mapper;

      public  RestauranteController(ObtenerDatosDelLocalCasoDeUso datosLocal, RestauranteMapper mapper)
      {
         _datosLocal = datosLocal;
         _mapper = mapper;
      }

      [HttpGet("{restauranteId:int}/configuracion-visual")]
      public async Task<ActionResult<RestauranteResponseDto>>ObtenerConfiguracionVisual(int restauranteId)
      {
         var restaurante =await _datosLocal.EjecutarAsync(restauranteId);

         if (restaurante is null) return NotFound();

         var dto = _mapper.aDto(restaurante);
         return Ok(dto);


      }


   }
}
