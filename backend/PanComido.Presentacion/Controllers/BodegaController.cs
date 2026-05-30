using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodegaController : ControllerBase
    {
        private readonly ListarBodegasConInsumosCasoDeUso _listarBodegasConInsumosCasoDeUso;
        private readonly ListarBodegasCasoDeUso _listarBodegasCasoDeUso;
        private readonly BodegaMapper _bodegaMapper;

        public BodegaController(ListarBodegasConInsumosCasoDeUso listarBodegasConInsumosCasoDeUso, 
            BodegaMapper bodegaMapper,
            ListarBodegasCasoDeUso listarBodegasCasoDeUso)
        {
            _listarBodegasConInsumosCasoDeUso = listarBodegasConInsumosCasoDeUso;
            _bodegaMapper = bodegaMapper;
            _listarBodegasCasoDeUso = listarBodegasCasoDeUso;
        }

        [HttpGet]
        public async Task<ActionResult<string>> obtener()
        {
            List<Bodega> bodegas = await _listarBodegasCasoDeUso.EjecutarAsync(ObtenerRestauranteId());

            return Ok(_bodegaMapper.bodegasAListaDto(bodegas));
        }

        [HttpGet("con-insumos")]
        public async Task<ActionResult<string>> obtenerConInsumos()
        {
            List<Bodega> bodegasConInsumos = await _listarBodegasConInsumosCasoDeUso.EjecutarAsync(ObtenerRestauranteId());

            return Ok(_bodegaMapper.bodegasConInsumosAListaDto(bodegasConInsumos));
        }

        // Helper temporal — luego lo pasaremos a id dinamico del restaurante
        private int ObtenerRestauranteId()
        {
            // TODO: reemplazar por: int.Parse(User.FindFirst("restauranteId")!.Value)
            return 1;
        }

    }
}
