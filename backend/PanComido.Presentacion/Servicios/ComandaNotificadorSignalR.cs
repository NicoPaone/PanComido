using Microsoft.AspNetCore.SignalR;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Presentacion.Hubs;
using System.Linq;

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
            List<ArticuloComanda> platos = comanda.Items.Where(i => i.Articulo is Plato).ToList();

            if (platos.Any())
            {
                Comanda comandaParaCocinaConSoloPlatos = new Comanda
                {
                    Id = comanda.Id,
                    MesaId = comanda.MesaId,
                    NumeroDeMesa = comanda.NumeroDeMesa,
                    RestauranteId = comanda.RestauranteId,
                    Estado = comanda.Estado,
                    CantComensales = comanda.CantComensales,
                    HoraInicio = comanda.HoraInicio,
                    HoraFin = comanda.HoraFin,
                    TiempoEstimadoTotal = comanda.Items
                    .Select(i => i.Articulo)
                    .OfType<Plato>()
                    .Select(plato => plato.TiempoPreparacionBase)
                    .DefaultIfEmpty(0)
                    .Max(),
                    HoraUltimoCambioEstado = comanda.HoraUltimoCambioEstado,
                    Items = platos
                };

                await _hubContext.Clients.Group($"Cocina_{comanda.RestauranteId}").SendAsync("EstadoComandaModificada", comandaParaCocinaConSoloPlatos);
            }

            await _hubContext.Clients.Group($"Mesa_{comanda.MesaId}").SendAsync("EstadoComandaModificada", comanda);

            foreach (int mozoId in mozoIds)
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

        public async Task NotificarComandaActualizadaAMesaAsync(Comanda comanda)
        {
            await _hubContext.Clients.Group($"Mesa_{comanda.MesaId}").SendAsync("ComandaActualizada", comanda);
        }

        public async Task NotificarPagoRechazadoAMesaAsync(Comanda comanda)
        {
            await _hubContext.Clients.Group($"Mesa_{comanda.MesaId}").SendAsync("PagoRechazado", comanda);
        }
    }
}