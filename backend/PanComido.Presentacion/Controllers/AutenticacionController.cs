using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso;
using PanComido.Presentacion.DTOs.Autenticacion;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Servicios;

namespace PanComido.Presentacion.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly LoginCasoDeUso _loginCasoDeUso;
        private readonly JwtTokenServicio _jwtTokenServicio;
        private readonly AutenticacionMapper _autenticacionMapper;

        public AutenticacionController(LoginCasoDeUso loginCasoDeUso, JwtTokenServicio jwtTokenServicio, AutenticacionMapper authMapper)
        {
            _loginCasoDeUso = loginCasoDeUso;
            _jwtTokenServicio = jwtTokenServicio;
            _autenticacionMapper = authMapper;
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var (empleado, rol) = await _loginCasoDeUso.EjecutarAsync(request.Email, request.Contrasenia);
            var token = _jwtTokenServicio.GenerarToken(
               empleado.Id,
               empleado.Email,
               empleado.Nombre,
               rol,
               empleado.RestauranteId
            );

            var response = _autenticacionMapper.aResponseDto(empleado, token, rol);
            return Ok(response);
        }
    }
}
