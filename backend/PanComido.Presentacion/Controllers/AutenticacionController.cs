using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso;
using PanComido.Presentacion.DTOs.Autenticacion;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Servicios;
using Microsoft.Extensions.Configuration;

namespace PanComido.Presentacion.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly LoginCasoDeUso _loginCasoDeUso;
        private readonly JwtTokenServicio _jwtTokenServicio;
        private readonly AutenticacionMapper _autenticacionMapper;
        private readonly SolicitarRecuperacionContraseniaCasoDeUso _solicitarRecuperacionCasoDeUso;
        private readonly EjecutarRecuperacionContraseniaCasoDeUso _ejecutarRecuperacionCasoDeUso;
        private readonly IConfiguration _configuration;

        public AutenticacionController(
            LoginCasoDeUso loginCasoDeUso, 
            JwtTokenServicio jwtTokenServicio, 
            AutenticacionMapper authMapper,
            SolicitarRecuperacionContraseniaCasoDeUso solicitarRecuperacionCasoDeUso,
            EjecutarRecuperacionContraseniaCasoDeUso ejecutarRecuperacionCasoDeUso,
            IConfiguration configuration)
        {
            _loginCasoDeUso = loginCasoDeUso;
            _jwtTokenServicio = jwtTokenServicio;
            _autenticacionMapper = authMapper;
            _solicitarRecuperacionCasoDeUso = solicitarRecuperacionCasoDeUso;
            _ejecutarRecuperacionCasoDeUso = ejecutarRecuperacionCasoDeUso;
            _configuration = configuration;
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

        [HttpPost("solicitar-recuperacion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitarResetDto request)
        {
            // Usamos el primer origen de CORS que generalmente es la URL del frontend.
            var urlFrontend = _configuration["CorsSettings:AllowedOrigins:0"] ?? "http://localhost:4200"; 
            await _solicitarRecuperacionCasoDeUso.EjecutarAsync(request.Email, urlFrontend);
            return Ok(new { Mensaje = "Si el correo es válido, se envió un enlace de recuperación." });
        }

        [HttpPost("ejecutar-recuperacion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EjecutarRecuperacion([FromBody] EjecutarResetDto request)
        {
            try
            {
                await _ejecutarRecuperacionCasoDeUso.EjecutarAsync(request.Email, request.Token, request.NuevaContrasenia);
                return Ok(new { Mensaje = "Contraseña actualizada exitosamente." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
