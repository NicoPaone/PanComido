using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ObtenerDatosTransferenciaCasoDeUso
    {
        private readonly IDatosTransferenciaRepositorio _datosTransferenciaRepositorio;
        public ObtenerDatosTransferenciaCasoDeUso(IDatosTransferenciaRepositorio datosTransferenciaRepositorio)
        {
            _datosTransferenciaRepositorio = datosTransferenciaRepositorio;
        }
        public async Task<DatosTransferencia> EjecutarAsync(int restauranteId)
        {
            var resultado = await _datosTransferenciaRepositorio.ObtenerDatosTransferenciaAsync(restauranteId);
            if (resultado == null) throw new KeyNotFoundException("Datos de transferencia no encontrados.");
            return resultado;
        }
    }
}
