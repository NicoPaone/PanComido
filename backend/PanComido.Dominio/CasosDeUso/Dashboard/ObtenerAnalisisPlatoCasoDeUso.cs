using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.IA;
using Microsoft.Extensions.Logging;
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
        private readonly ICalculadorCostoPlatoServicio _calculadorCostoPlatoServicio;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISugerenciaPlatosIAServicio _sugerenciaPlatosIAServicio;
        private readonly ILogger<ObtenerAnalisisPlatoCasoDeUso> _logger;

        public ObtenerAnalisisPlatoCasoDeUso(
            IPlatoAnalisisRepositorio platoAnalisisRepositorio,
            ISugerenciaIARepositorio sugerenciaIARepositorio,
            ICalculadorCostoPlatoServicio calculadorCostoPlatoServicio,
            IDateTimeProvider dateTimeProvider,
            ISugerenciaPlatosIAServicio sugerenciaPlatosIAServicio,
            ILogger<ObtenerAnalisisPlatoCasoDeUso> logger)
        {
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
            _sugerenciaIARepositorio = sugerenciaIARepositorio;
            _calculadorCostoPlatoServicio = calculadorCostoPlatoServicio;
            _dateTimeProvider = dateTimeProvider;
            _sugerenciaPlatosIAServicio = sugerenciaPlatosIAServicio;
            _logger = logger;
        }


        public async Task<PlatoAnalisisResultado?> EjecutarAsync(int restauranteId, string nombrePlato)
        {
            var articulo = await _platoAnalisisRepositorio.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato);
            var plato = articulo as Plato;
            if (plato == null)
            {
                return null;
            }

            decimal costoPreparacion = await _calculadorCostoPlatoServicio.CalcularCostoAsync(plato);

            DateTime hoy = _dateTimeProvider.ObtenerAhora();
            DateTime desde30 = _dateTimeProvider.ObtenerHoy().AddDays(-30);
            DateTime desde60 = _dateTimeProvider.ObtenerHoy().AddDays(-60);

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

            int totalVentasCategoria = await _platoAnalisisRepositorio.ObtenerVentasCategoriaEnRangoAsync(restauranteId, plato.CategoriaPlatoId, desde30, hoy);
            string participacion = "0%";
            if (totalVentasCategoria > 0)
            {
                decimal partPct = (decimal)ventasPeriodo / totalVentasCategoria * 100;
                participacion = $"{partPct:N1}%";
            }

            var lider = await _platoAnalisisRepositorio.ObtenerPlatoLiderDeCategoriaAsync(restauranteId, plato.CategoriaPlatoId, desde30, hoy);
            RendimientoPlato comparativaLider = lider ?? new RendimientoPlato
            {
                Nombre = "No disponible",
                UnidadesVendidas = 0,
                FacturacionTotal = 0
            };

            DateTime desdeSemana = _dateTimeProvider.ObtenerHoy().AddDays(-49);
            var tendencia = await _platoAnalisisRepositorio.ObtenerVentasSemanalesArticuloAsync(restauranteId, plato.Id, desdeSemana, _dateTimeProvider.ObtenerHoy());

            var sugerenciaIa = await _sugerenciaIARepositorio.ObtenerSugerenciaIAAsync(restauranteId);
            if (sugerenciaIa == null)
            {
                sugerenciaIa = new SugerenciaIA
                {
                    FechaSugerencia = DateTime.MinValue,
                    FechaUltimoAnalisisIA = _dateTimeProvider.ObtenerAhora(),
                    PlatosAnalisis = new List<PlatoAnalisisIa>()
                };
            }
            else if (!sugerenciaIa.FechaUltimoAnalisisIA.HasValue || sugerenciaIa.FechaUltimoAnalisisIA.Value.Date != _dateTimeProvider.ObtenerHoy())
            {
                sugerenciaIa.FechaUltimoAnalisisIA = _dateTimeProvider.ObtenerAhora();
                sugerenciaIa.PlatosAnalisis = new List<PlatoAnalisisIa>();
            }

            if (sugerenciaIa.PlatosAnalisis == null)
            {
                sugerenciaIa.PlatosAnalisis = new List<PlatoAnalisisIa>();
            }

            var analisisPlato = sugerenciaIa.PlatosAnalisis.FirstOrDefault(p => p.PlatoId == plato.Id);
            bool analisisProvieneDeCache = analisisPlato != null;
            string? motivoFallback = null;
            if (analisisPlato == null)
            {
                if (ventasPeriodo > 0)
                {
                    try
                    {
                        analisisPlato = await _sugerenciaPlatosIAServicio.AnalizarPlatoRendimientoAsync(
                            plato,
                            costoPreparacion,
                            ventasPeriodo,
                            volumenVar,
                            participacion,
                            comparativaLider,
                            tendencia
                        );
                        if (analisisPlato != null)
                        {
                            analisisPlato.PlatoId = plato.Id;
                            analisisPlato.Nombre = plato.Nombre;
                            analisisPlato.FuenteAnalisis = "ia";
                            analisisPlato.EsFallbackLocal = false;
                            analisisPlato.MotivoFallback = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "No se pudo obtener analisis IA para el plato {PlatoId} del restaurante {RestauranteId}. Se usara fallback local.",
                            plato.Id,
                            restauranteId);
                        analisisPlato = null;
                        motivoFallback = "No se pudo obtener analisis de IA en este momento.";
                    }
                }
                else
                {
                    motivoFallback = "No hay ventas suficientes para solicitar analisis de IA.";
                }

                if (analisisPlato == null)
                {
                    analisisPlato = GenerarAnalisisLocalRespaldo(plato, motivoFallback);
                }

                if (!analisisPlato.EsFallbackLocal)
                {
                    sugerenciaIa.PlatosAnalisis.Add(analisisPlato);
                    await _sugerenciaIARepositorio.GuardarSugerenciaIAAsync(restauranteId, sugerenciaIa);
                }
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
                AnalisisIa = analisisPlato,
                AnalisisProvieneDeCache = analisisProvieneDeCache,
                FuenteAnalisis = analisisPlato.FuenteAnalisis,
                EsFallbackLocal = analisisPlato.EsFallbackLocal,
                MotivoFallback = analisisPlato.MotivoFallback
            };
        }

        private PlatoAnalisisIa GenerarAnalisisLocalRespaldo(Plato plato, string? motivoFallback)
        {
            return new PlatoAnalisisIa
            {
                PlatoId = plato.Id,
                Nombre = plato.Nombre,
                FuenteAnalisis = "fallback_local",
                EsFallbackLocal = true,
                MotivoFallback = motivoFallback ?? "La IA no devolvio un analisis disponible.",
                Diagnostico = $"El precio actual de {plato.Nombre} está afectando su rotación debido al incremento en el costo de los insumos primarios.",
                Alerta = PanComido.Dominio.Entidades.Enums.CriticidadAlerta.Critica.ToString().ToLower(),
                Sugerencias = new List<PlatoSugerenciaIa>
                {
                    new PlatoSugerenciaIa
                    {
                        Id = 1,
                        Tipo = PanComido.Dominio.Entidades.Enums.TipoSugerencia.Descuento.ToString().ToLower(),
                        Accion = $"Aplicar descuento promocional del 10% por 1 semana a {plato.Nombre}.",
                        Impacto = "Impacto Medio (+10 u./mes)",
                        Dificultad = PanComido.Dominio.Entidades.Enums.DificultadSugerencia.Baja.ToString().ToLower(),
                        EsAplicable = true,
                        Aplicada = false
                    },
                    new PlatoSugerenciaIa
                    {
                        Id = 2,
                        Tipo = PanComido.Dominio.Entidades.Enums.TipoSugerencia.Destacar.ToString().ToLower(),
                        Accion = $"Destacar {plato.Nombre} en el menú para darle mayor visibilidad.",
                        Impacto = "Impacto Alto (+20 u./mes)",
                        Dificultad = PanComido.Dominio.Entidades.Enums.DificultadSugerencia.Baja.ToString().ToLower(),
                        EsAplicable = true,
                        Aplicada = false
                    }
                }
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
        public bool AnalisisProvieneDeCache { get; set; }
        public string FuenteAnalisis { get; set; } = "desconocida";
        public bool EsFallbackLocal { get; set; }
        public string? MotivoFallback { get; set; }
    }
}
