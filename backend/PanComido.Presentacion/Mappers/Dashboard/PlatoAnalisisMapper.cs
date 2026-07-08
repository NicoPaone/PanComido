using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Presentacion.DTOs.Dashboard;
using System.Collections.Generic;
using System.Linq;

namespace PanComido.Presentacion.Mappers.Dashboard
{
    public class PlatoAnalisisMapper
    {
        public PlatoAnalisisDto ParaDto(PlatoAnalisisResultado resultado)
        {
            var dto = new PlatoAnalisisDto
            {
                PlatoId = resultado.Articulo.Id,
                Plato = new DashboardRankingItemDto
                {
                    Nombre = resultado.Articulo.Nombre,
                    Valor = resultado.VentasPeriodo,
                    Detalle = $"$ {resultado.Articulo.PrecioVentaFinal:N0}"
                },
                Diagnostico = resultado.AnalisisIa.Diagnostico,
                Alerta = resultado.AnalisisIa.Alerta,
                FuenteAnalisis = resultado.FuenteAnalisis,
                EsFallbackLocal = resultado.EsFallbackLocal,
                AnalisisProvieneDeCache = resultado.AnalisisProvieneDeCache,
                MensajeFallback = resultado.MotivoFallback,
                Metricas = new MetricasAnalisisDto
                {
                    Volumen = $"{resultado.VentasPeriodo} u.",
                    VolumenVar = resultado.VolumenVar,
                    Costo = $"$ {resultado.CostoPreparacion:N0}",
                    Precio = $"$ {resultado.Articulo.PrecioVentaFinal:N0}",
                    MargenPct = resultado.Articulo.PrecioVentaFinal > 0
                        ? $"{((resultado.Articulo.PrecioVentaFinal.Value - resultado.CostoPreparacion) / resultado.Articulo.PrecioVentaFinal.Value * 100):N0}%"
                        : "0%",
                    Participacion = resultado.Participacion
                },
                Comparativa = new ComparativaAnalisisDto
                {
                    Nombre = resultado.ComparativaLider.Nombre,
                    Precio = $"$ {(resultado.ComparativaLider.UnidadesVendidas > 0 ? (resultado.ComparativaLider.FacturacionTotal / resultado.ComparativaLider.UnidadesVendidas) : 0m):N0}",
                    Ventas = $"{resultado.ComparativaLider.UnidadesVendidas} u."
                },
                Tendencia = resultado.Tendencia,
                SugerenciasDetalladas = ParaListaDto(resultado.AnalisisIa.Sugerencias)
            };

            return dto;
        }

        public PlatoSugerenciaDto ParaDto(PlatoSugerenciaIa sugerenciaIa)
        {
            return new PlatoSugerenciaDto
            {
                Id = sugerenciaIa.Id,
                Tipo = sugerenciaIa.Tipo,
                Accion = sugerenciaIa.Accion,
                Impacto = sugerenciaIa.Impacto,
                Dificultad = sugerenciaIa.Dificultad,
                EsAplicable = sugerenciaIa.EsAplicable,
                Aplicada = sugerenciaIa.Aplicada
            };
        }

        public List<PlatoSugerenciaDto> ParaListaDto(List<PlatoSugerenciaIa> sugerenciasIa)
        {
            return sugerenciasIa.Select(ParaDto).ToList();
        }
    }
}
