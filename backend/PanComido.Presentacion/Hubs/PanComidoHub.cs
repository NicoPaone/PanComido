using Microsoft.AspNetCore.SignalR;

namespace PanComido.Presentacion.Hubs
{
    public class PanComidoHub : Hub
    {
        public async Task UnirseCocina(int restauranteId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Cocina_{restauranteId}");
        }

        public async Task UnirseMozos(int restauranteId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Mozos_{restauranteId}");
        }

        public async Task UnirseMozo(int restauranteId, int mozoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Mozos_{restauranteId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Mozo_{mozoId}");
        }

        public async Task UnirseGerente(int restauranteId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Gerente_{restauranteId}");
        }

        public async Task UnirseMesa(int mesaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Mesa_{mesaId}");
        }
    }
}