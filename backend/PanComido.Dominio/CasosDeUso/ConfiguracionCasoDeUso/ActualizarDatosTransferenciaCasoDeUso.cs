using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarDatosTransferenciaCasoDeUso
    {
        private readonly IDatosTransferenciaRepositorio _datosTransferenciaRepositorio;
        private readonly ILogger<ActualizarDatosTransferenciaCasoDeUso> _logger;

        public ActualizarDatosTransferenciaCasoDeUso(IDatosTransferenciaRepositorio datosTransferenciaRepositorio, ILogger<ActualizarDatosTransferenciaCasoDeUso> logger)
        {
            _datosTransferenciaRepositorio = datosTransferenciaRepositorio;
            _logger = logger;
        }

        public async Task<DatosTransferencia> EjecutarAsync(int restauranteId, DatosTransferencia datosTransferencia)
        {
            if(datosTransferencia.Alias == null || datosTransferencia.NumeroCuenta == null || datosTransferencia.TitularCuenta == null)
            {
                _logger.LogWarning("Datos de transferencia incompletos. RestauranteId: {RestauranteId}", restauranteId);
                throw new ArgumentException("El alias, numero de cuenta y titular no pueden estar vacios.");
            }

            if(datosTransferencia.Cbu != null && datosTransferencia.Cbu.Length != 22)
            {
                _logger.LogWarning("CBU inválido. RestauranteId: {RestauranteId}, CBU: {CBU}", restauranteId, datosTransferencia.Cbu);
                throw new ArgumentException("El CBU debe tener 22 caracteres.");
            }

            var resultado = await _datosTransferenciaRepositorio.ActualizarDatosTransferenciaAsync(restauranteId, datosTransferencia);
            _logger.LogInformation("Datos de transferencia actualizados. RestauranteId: {RestauranteId}", restauranteId);

            return resultado;
        }
    }
}
