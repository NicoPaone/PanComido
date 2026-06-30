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

            string jsonDesc = System.Text.Json.JsonSerializer.Serialize(new
            {
                titulo = $"Revisión: {plato.Nombre}",
                detalle = $"Medir impacto de: {accionSugerida}"
            }, new System.Text.Json.JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            await _platoAnalisisRepositorio.GuardarRecordatorioNotificacionAsync(restauranteId, jsonDesc);

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
                Destino = PanComido.Dominio.Entidades.Enums.NotificacionDestino.Carta.ToString().ToLower(),
                Tono = PanComido.Dominio.Entidades.Enums.NotificacionTono.Info.ToString().ToLower(),
                Impacto = "Reevaluar demanda",
                Prioridad = 4
            };
        }
    }

    public class AgendarRecordatorioResultado
    {
        public string Mensaje { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public string Destino { get; set; } = "carta";
        public string Tono { get; set; } = "info";
        public string Impacto { get; set; } = "Reevaluar demanda";
        public int Prioridad { get; set; } = 4;
    }
}

