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
            await _hubContext.Clients.Group($"Mozos_{llamado.MozoId}").SendAsync("LlamadoMozo", llamado);
        }
    }
}
