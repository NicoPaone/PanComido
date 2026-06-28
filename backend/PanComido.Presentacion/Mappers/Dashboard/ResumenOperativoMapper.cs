using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Dashboard;
using System.Linq;

namespace PanComido.Presentacion.Mappers.Dashboard
{
    public static class ResumenOperativoMapper
    {
        public static ResumenOperativoResponseDto ParaDto(ResumenOperativo dominio)
        {
            return new ResumenOperativoResponseDto
            {
                TotalVentas = $"$ {dominio.TotalVentas:N0}",
                TotalPedidos = dominio.TotalPedidos,
                TicketPromedio = $"$ {dominio.TicketPromedio:N0}",
                PromedioDiarioPedidos = dominio.PromedioDiarioPedidos,

                VariacionVentas = FormatearPorcentaje(dominio.VariacionVentas),
                VariacionPedidos = FormatearPorcentaje(dominio.VariacionPedidos),
                VariacionTicket = FormatearPorcentaje(dominio.VariacionTicket),

                Grafico = dominio.Grafico.Select(g => new VentaAgrupadaDto
                {
                    Etiqueta = g.Etiqueta,
                    Total = $"$ {g.Total:N0}"
                }).ToList(),

                Recordatorios = dominio.Recordatorios.Select(r => new DashboardAccionItemDto
                {
                    Id = r.Id,
                    Titulo = r.Titulo,
                    Detalle = r.Detalle,
                    Destino = r.Destino,
                    Tono = r.Tono,
                    Impacto = r.Impacto,
                    Prioridad = r.Prioridad
                }).ToList(),

                Mozos = dominio.Mozos.Select(m => new EstadisticaMozoDto
                {
                    Nombre = m.Nombre,
                    MesasAtendidas = m.MesasAtendidas,
                    FacturacionTotal = m.FacturacionTotal,
                    TiempoPromedioAtencion = m.TiempoPromedioAtencion,
                    Estado = m.Estado
                }).ToList()
            };
        }

        private static string FormatearPorcentaje(decimal valor)
        {
            if (valor > 0) return $"+{valor:N1}%";
            if (valor < 0) return $"{valor:N1}%";
            return "0.0%";
        }
    }
}
