using Microsoft.AspNetCore.SignalR;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Presentacion.Hubs;
using PanComido.Presentacion.Mappers;

namespace PanComido.Presentacion.Servicios
{
    public class MesaNotificadorSignalR : IMesaNotificador
    {
        private readonly IHubContext<PanComidoHub> _hubContext;
        private readonly MesaMapper _mesaMapper;

        public MesaNotificadorSignalR(IHubContext<PanComidoHub> hubContext, MesaMapper mesaMapper)
        {
            _hubContext = hubContext;
            _mesaMapper = mesaMapper;
        }

        public async Task NotificarMesaActualizadaAsync(MesaConPosiciones mesa, int restauranteId)
        {
            var mesaDto = _mesaMapper.aDto(mesa);
            await _hubContext.Clients.Group($"Gerente_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
            await _hubContext.Clients.Group($"Mozos_{restauranteId}").SendAsync("MesaActualizada", mesaDto);
        }
    }
}
