using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerAnalisisPlatoCasoDeUso
    {
        private readonly IPlatoAnalisisRepositorio _platoAnalisisRepositorio;
        private readonly ISugerenciaIARepositorio _sugerenciaIARepositorio;

        public ObtenerAnalisisPlatoCasoDeUso(
            IPlatoAnalisisRepositorio platoAnalisisRepositorio,
            ISugerenciaIARepositorio sugerenciaIARepositorio)
        {
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
            _sugerenciaIARepositorio = sugerenciaIARepositorio;
        }

        public async Task<PlatoAnalisisResultado?> EjecutarAsync(int restauranteId, string nombrePlato)
        {
            var articulo = await _platoAnalisisRepositorio.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato);
            var plato = articulo as Plato;
            if (plato == null)
            {
                return null;
            }

            // 1. Costo dinámico
            decimal costoPreparacion = 0;
            if (plato.Ingredientes != null)
            {
                foreach (var item in plato.Ingredientes)
                {
                    decimal ultimoPrecioCompra = await _platoAnalisisRepositorio.ObtenerUltimoPrecioCompraInsumoAsync(item.InsumoId);
                    costoPreparacion += item.Cantidad * (ultimoPrecioCompra > 0 ? ultimoPrecioCompra : 0);
                }
            }

            // 2. Ventas del periodo y variación vs mes anterior
            DateTime hoy = DateTime.Now;
            DateTime desde30 = DateTime.Today.AddDays(-30);
            DateTime desde60 = DateTime.Today.AddDays(-60);

            int ventasPeriodo = await _platoAnalisisRepositorio.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, desde30, hoy);
            int ventasPeriodoAnterior = await _platoAnalisisRepositorio.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, desde60, desde30);

            string volumenVar;
            if (ventasPeriodoAnterior == 0)
            {
                volumenVar = ventasPeriodo > 0 ? "+100% vs mes anterior" : "0% vs mes anterior";
            }
            else
            {
                decimal varPct = ((decimal)ventasPeriodo - ventasPeriodoAnterior) / ventasPeriodoAnterior * 100;
                string signo = varPct >= 0 ? "+" : "";
                volumenVar = $"{signo}{varPct:N0}% vs mes anterior";
            }

            // 3. Participación en su categoría
            int totalVentasCategoria = await _platoAnalisisRepositorio.ObtenerVentasCategoriaEnRangoAsync(restauranteId, plato.CategoriaPlatoId, desde30, hoy);
            string participacion = "0%";
            if (totalVentasCategoria > 0)
            {
                decimal partPct = (decimal)ventasPeriodo / totalVentasCategoria * 100;
                participacion = $"{partPct:N1}%";
            }

            // 4. Comparativa con líder de la categoría
            var lider = await _platoAnalisisRepositorio.ObtenerPlatoLiderDeCategoriaAsync(restauranteId, plato.CategoriaPlatoId, desde30, hoy);
            RendimientoPlato comparativaLider = lider ?? new RendimientoPlato
            {
                Nombre = "No disponible",
                UnidadesVendidas = 0,
                FacturacionTotal = 0
            };

            // 5. Tendencia (ventas semanales de las últimas 7 semanas)
            var tendencia = new List<int>();
            for (int i = 6; i >= 0; i--)
            {
                DateTime desdeSemana = DateTime.Today.AddDays(-7 * (i + 1));
                DateTime hastaSemana = DateTime.Today.AddDays(-7 * i);
                int ventasSemana = await _platoAnalisisRepositorio.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, desdeSemana, hastaSemana);
                tendencia.Add(ventasSemana);
            }

            // 6. Sugerencias e IA
            var sugerenciaIa = await _sugerenciaIARepositorio.ObtenerSugerenciaIAAsync(restauranteId);
            if (sugerenciaIa == null)
            {
                sugerenciaIa = new SugerenciaIA
                {
                    FechaSugerencia = DateTime.Now,
                    PlatosAnalisis = new List<PlatoAnalisisIa>()
                };
            }
            if (sugerenciaIa.PlatosAnalisis == null)
            {
                sugerenciaIa.PlatosAnalisis = new List<PlatoAnalisisIa>();
            }

            var analisisPlato = sugerenciaIa.PlatosAnalisis.FirstOrDefault(p => p.PlatoId == plato.Id);
            if (analisisPlato == null)
            {
                // Inicializar sugerencias por defecto si no existen
                analisisPlato = new PlatoAnalisisIa
                {
                    PlatoId = plato.Id,
                    Nombre = plato.Nombre,
                    Diagnostico = $"El precio actual de {plato.Nombre} está afectando su rotación debido al incremento en el costo de los insumos primarios.",
                    Alerta = "critica",
                    Sugerencias = new List<PlatoSugerenciaIa>
                    {
                        new PlatoSugerenciaIa
                        {
                            Id = 1,
                            Tipo = "descuento",
                            Accion = $"Aplicar descuento promocional del 10% por 1 semana a {plato.Nombre}.",
                            Impacto = "Impacto Medio (+10 u./mes)",
                            Dificultad = "baja",
                            EsAplicable = true,
                            Aplicada = false
                        },
                        new PlatoSugerenciaIa
                        {
                            Id = 2,
                            Tipo = "combo",
                            Accion = $"Ofrecer {plato.Nombre} en combo promocional con Bebida.",
                            Impacto = "Impacto Alto (+20 u./mes)",
                            Dificultad = "media",
                            EsAplicable = true,
                            Aplicada = false
                        }
                    }
                };
                sugerenciaIa.PlatosAnalisis.Add(analisisPlato);
                await _sugerenciaIARepositorio.GuardarSugerenciaIAAsync(restauranteId, sugerenciaIa);
            }

            return new PlatoAnalisisResultado
            {
                Articulo = plato,
                CostoPreparacion = costoPreparacion,
                VentasPeriodo = ventasPeriodo,
                VolumenVar = volumenVar,
                Participacion = participacion,
                ComparativaLider = comparativaLider,
                Tendencia = tendencia,
                AnalisisIa = analisisPlato
            };
        }
    }

    public class PlatoAnalisisResultado
    {
        public Articulo Articulo { get; set; } = null!;
        public decimal CostoPreparacion { get; set; }
        public int VentasPeriodo { get; set; }
        public string VolumenVar { get; set; } = string.Empty;
        public string Participacion { get; set; } = string.Empty;
        public RendimientoPlato ComparativaLider { get; set; } = null!;
        public List<int> Tendencia { get; set; } = new List<int>();
        public PlatoAnalisisIa AnalisisIa { get; set; } = null!;
    }
}
