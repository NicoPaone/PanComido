using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class AgendarRecordatorioCasoDeUso
    {
        private readonly IArticuloRepositorio _articuloRepositorio;
        private readonly ISugerenciaIARepositorio _sugerenciaIARepositorio;
        private readonly IPlatoAnalisisRepositorio _platoAnalisisRepositorio;

        public AgendarRecordatorioCasoDeUso(
            IArticuloRepositorio articuloRepositorio,
            ISugerenciaIARepositorio sugerenciaIARepositorio,
            IPlatoAnalisisRepositorio platoAnalisisRepositorio)
        {
            _articuloRepositorio = articuloRepositorio;
            _sugerenciaIARepositorio = sugerenciaIARepositorio;
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
        }

        public async Task<AgendarRecordatorioResultado?> EjecutarAsync(int restauranteId, int platoId, string accionSugerida)
        {
            var articulo = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, platoId);
            var plato = articulo as Plato;
            if (plato == null)
            {
                return null;
            }

            // 1. Agendar recordatorio (notificación)
            string descripcion = $"Revisión: {plato.Nombre} - Medir impacto de: {accionSugerida}";
            await _platoAnalisisRepositorio.GuardarRecordatorioNotificacionAsync(restauranteId, descripcion);

            // 2. Modificar el estado de la sugerencia a aplicada
            var sugerenciaIa = await _sugerenciaIARepositorio.ObtenerSugerenciaIAAsync(restauranteId);
            if (sugerenciaIa != null && sugerenciaIa.PlatosAnalisis != null)
            {
                var analisisPlato = sugerenciaIa.PlatosAnalisis.FirstOrDefault(p => p.PlatoId == platoId);
                if (analisisPlato != null)
                {
                    var sug = analisisPlato.Sugerencias
                        .FirstOrDefault(s => s.Accion.Equals(accionSugerida, StringComparison.OrdinalIgnoreCase));
                    
                    if (sug == null)
                    {
                        // Fallback: buscar por tipo si la acción no coincide exactamente
                        sug = analisisPlato.Sugerencias.FirstOrDefault(s => s.Tipo == "combo");
                    }

                    if (sug != null)
                    {
                        sug.Aplicada = true;
                    }
                    await _sugerenciaIARepositorio.GuardarSugerenciaIAAsync(restauranteId, sugerenciaIa);
                }
            }

            return new AgendarRecordatorioResultado
            {
                Mensaje = "Recordatorio guardado en el módulo de tareas administrativas.",
                Titulo = $"Revisión: {plato.Nombre}",
                Detalle = $"Medir impacto de: {accionSugerida}",
                Destino = "carta",
                Tono = "info",
                Impacto = "Reevaluar demanda",
                Prioridad = 4
            };
        }
    }

    public class AgendarRecordatorioResultado
    {
        public string Mensaje { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty; // Detalle mapped to Detail to keep consistency or we can use Detalle
        public string Detalle { get; set; } = string.Empty;
        public string Destino { get; set; } = "carta";
        public string Tono { get; set; } = "info";
        public string Impacto { get; set; } = "Reevaluar demanda";
        public int Prioridad { get; set; } = 4;
    }
}
