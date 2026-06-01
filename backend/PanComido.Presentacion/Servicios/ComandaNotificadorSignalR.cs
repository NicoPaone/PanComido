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

        public async Task NotificarEstadoModificadoAsync(Comanda comanda)
        {
           await _hubContext.Clients.Group($"Cocina_{comanda.RestauranteId}").SendAsync("EstadoComandaModificada", comanda);
           await _hubContext.Clients.Group($"Mozos_{comanda.RestauranteId}").SendAsync("EstadoComandaModificada", comanda);
        }
    }
}
