using PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IFilaVirtualNotificador
    {
        Task NotificarEstadoActualizadoAsync(int turnoId, EstadoFilaMesaResult estado);
        Task NotificarMesaListaAsync(int turnoId, int mesaId, int minutosParaOcupar);
        Task NotificarTurnoExpiradoAsync(int turnoId, string mensajeExpulsion);
    }
}
