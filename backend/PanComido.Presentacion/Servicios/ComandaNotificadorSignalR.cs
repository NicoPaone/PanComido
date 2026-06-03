using Microsoft.AspNetCore.SignalR;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Presentacion.Hubs;

namespace PanComido.Presentacion.Servicios
{
    public class ComandaNotificadorSignalR : IComandaNotificador
    {
        private readonly IHubContext<PanComidoHub> _hubContext;

        public ComandaNotificadorSignalR(IHubContext<PanComidoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotificarEstadoModificadoAsync(Comanda comanda, List<int> mozoIds)
        {
           await _hubContext.Clients.Group($"Cocina_{comanda.RestauranteId}").SendAsync("EstadoComandaModificada", comanda);
            foreach(int mozoId in mozoIds)
            {
                await _hubContext.Clients.Group($"Mozo_{mozoId}").SendAsync("EstadoComandaModificada", comanda);
            }
        }

        public async Task NotificarLlamadoCocinaAsync(Comanda comanda, List<int> mozoIds)
        {
            foreach (var mozoId in mozoIds)
            {
                await _hubContext.Clients.Group($"Mozo_{mozoId}").SendAsync("LlamadoCocina", comanda);
            }
        }
    }
}
