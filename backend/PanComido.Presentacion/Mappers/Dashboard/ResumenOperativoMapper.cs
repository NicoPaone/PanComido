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
                }).ToList()
            };
        }

        private static string FormatearPorcentaje(decimal valor)
        {
            if (valor > 0) return $"+{valor:N1}%";
            if (valor < 0) return $"{valor:N1}%"; // El negativo ya viene incluido
            return "0.0%";
        }
    }
}
