using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.CierreCajaCasoDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.CierreCaja;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("cierre-caja")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class CierreCajaController : ControllerBase
    {
        private readonly GenerarCierreDeCajaCasoDeUso _generarCierreDeCajaCasoDeUso;
        private readonly ListarCierresDeCajaCasoDeUso _listarCierresDeCajaCasoDeUso;
        private readonly ObtenerDetalleCierreCasoDeUso _obtenerDetalleCierreCasoDeUso;
        private readonly CierreCajaMapper _cierreCajaMapper;

        public CierreCajaController(
            GenerarCierreDeCajaCasoDeUso generarCierreDeCajaCasoDeUso,
            ListarCierresDeCajaCasoDeUso listarCierresDeCajaCasoDeUso,
            ObtenerDetalleCierreCasoDeUso obtenerDetalleCierreCasoDeUso,
            CierreCajaMapper cierreCajaMapper)
        {
            _generarCierreDeCajaCasoDeUso = generarCierreDeCajaCasoDeUso;
            _listarCierresDeCajaCasoDeUso = listarCierresDeCajaCasoDeUso;
            _obtenerDetalleCierreCasoDeUso = obtenerDetalleCierreCasoDeUso;
            _cierreCajaMapper = cierreCajaMapper;
        }

        [HttpPost("generar")]
        [ProducesResponseType(typeof(CierreCajaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CierreCajaResponseDto>> Generar([FromBody] CierreCajaRequestDto request)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var cierre = await _generarCierreDeCajaCasoDeUso.EjecutarAsync(restauranteId, request.IdTurnoLaboral, request.ConteoCaja);

            return Ok(await ArmarDtoAsync(restauranteId, cierre));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CierreCajaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CierreCajaResponseDto>>> Listar()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var cierres = await _listarCierresDeCajaCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = new List<CierreCajaResponseDto>();
            foreach (var cierre in cierres)
            {
                dtos.Add(await ArmarDtoAsync(restauranteId, cierre));
            }

            return Ok(dtos);
        }

        private async Task<CierreCajaResponseDto> ArmarDtoAsync(int restauranteId, Cierre cierre)
        {
            var detalle = await _obtenerDetalleCierreCasoDeUso.EjecutarAsync(restauranteId, cierre);
            var turnoNombre = detalle.Turno.EsNocturno ? "Turno Noche" : "Turno Día";

            return _cierreCajaMapper.aDto(cierre, turnoNombre, detalle.CantidadTotalDePagos, detalle.TotalRecaudado, detalle.ResumenPorMetodo);
        }
    }
}
