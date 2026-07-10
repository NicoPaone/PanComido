using Microsoft.AspNetCore.SignalR;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados;
using PanComido.Presentacion.Hubs;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Servicios
{
    public class FilaVirtualNotificadorSignalR : IFilaVirtualNotificador
    {
        private readonly IHubContext<PanComidoHub> _hubContext;

        public FilaVirtualNotificadorSignalR(IHubContext<PanComidoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotificarEstadoActualizadoAsync(int turnoId, EstadoFilaMesaResult estado)
        {
            await _hubContext.Clients.Group($"TurnoFila_{turnoId}").SendAsync("EstadoFilaActualizado", estado);
        }

        public async Task NotificarMesaListaAsync(int turnoId, int mesaId, int minutosParaOcupar)
        {
            await _hubContext.Clients.Group($"TurnoFila_{turnoId}").SendAsync("MesaListaParaOcupar", new { mesaId, minutosParaOcupar });
        }

        public async Task NotificarTurnoExpiradoAsync(int turnoId, string mensajeExpulsion)
        {
            await _hubContext.Clients.Group($"TurnoFila_{turnoId}").SendAsync("TurnoExpirado", mensajeExpulsion);
        }
    }
}
