using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IDatosTransferenciaRepositorio
    {
        Task<DatosTransferencia?> ObtenerDatosTransferenciaAsync(int restauranteId);
        Task<DatosTransferencia> ActualizarDatosTransferenciaAsync(int restauranteId, DatosTransferencia datosTransferencia);
    }
}
