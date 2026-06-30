using Microsoft.AspNetCore.SignalR;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Presentacion.Hubs;

namespace PanComido.Presentacion.Servicios
{
    public class LlamadoNotificadorSignalR : ILlamadoNotificador
    {
        private readonly IHubContext<PanComidoHub> _hubContext;

        public LlamadoNotificadorSignalR(IHubContext<PanComidoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotificarLlamadoAsync(Llamado llamado)
        {
            await _hubContext.Clients.Group($"Mozo_{llamado.MozoId}").SendAsync("LlamadoMozo", llamado);
        }

        public async Task NotificarLlamadosResueltosAsync(int mesaId, List<Llamado> llamadosResueltos)
        {
            List<int> mozosId = llamadosResueltos.Where(l => l.MozoId.HasValue).Select(l => l.MozoId!.Value).Distinct().ToList();

            foreach (int mozoId in mozosId)
            {
                await _hubContext.Clients.Group($"Mozo_{mozoId}").SendAsync("LlamadosResueltosAutomaticamente", new { mesaId, llamadoIds = llamadosResueltos.Select(l => l.Id).ToList() });
            }
        }
    }
}
