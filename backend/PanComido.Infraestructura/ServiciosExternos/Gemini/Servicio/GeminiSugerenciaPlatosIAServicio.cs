using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios.IA;
using PanComido.Infraestructura.ServiciosExternos.Gemini;
using PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Request;
using PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response;
using PanComido.Infraestructura.ServiciosExternos.Gemini.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.Servicio
{
    public class GeminiSugerenciaPlatosIAServicio : ISugerenciaPlatosIAServicio
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiResponseMapper _mapper;
        private readonly GeminiConfiguracion _configuracion;

        public GeminiSugerenciaPlatosIAServicio(HttpClient httpClient,
                                                GeminiResponseMapper mapper,
                                                IOptions<GeminiConfiguracion> configuracion)
        {
            _httpClient = httpClient;
            _mapper = mapper;
            _configuracion = configuracion.Value;
        }

        public async Task<SugerenciaIA> GenerarSugerenciasAsync(int restauranteId,
                                                                List<Insumo> insumosDisponibles, 
                                                                Dictionary<int, List<Lote>> vencimientosProximos,
                                                                List<string> platosExistentes,
                                                                int cantidadPlatos)
        {
            string prompt = ConstruirPrompt(insumosDisponibles,
                                            vencimientosProximos,
                                            platosExistentes,
                                            cantidadPlatos);

            string apiKey = _configuracion.ApiKey;
            string url = _configuracion.Url;

            GeminiRequestDto requestDto = CrearRequest(prompt);

            var response = await _httpClient.PostAsJsonAsync($"{url}?key={apiKey}", requestDto);

            response.EnsureSuccessStatusCode();
            string jsonRespuesta = await response.Content.ReadAsStringAsync();

            GeminiApiResponseDto? respuestaGemini = JsonSerializer.Deserialize<GeminiApiResponseDto>(jsonRespuesta);

            if (respuestaGemini == null)
            {
                throw new Exception("No se pudo deserializar la respuesta de Gemini.");
            }

            string jsonRespuestaTest = await response.Content.ReadAsStringAsync();

            Console.WriteLine(jsonRespuestaTest);

            string textoRespuesta = respuestaGemini?.Candidatos
                                                    .FirstOrDefault()?
                                                    .Contenido
                                                    .Partes
                                                    .FirstOrDefault()?
                                                    .Texto
                                                    ?? throw new Exception("Gemini no devolvió contenido.");

            // Gemini a veces envuelve el JSON en ```json ... ``` aunque se le pida que no
            textoRespuesta = textoRespuesta.Trim();
            if (textoRespuesta.StartsWith("```"))
            {
                int inicio = textoRespuesta.IndexOf('\n') + 1;
                int fin = textoRespuesta.LastIndexOf("```");
                textoRespuesta = textoRespuesta[inicio..fin].Trim();
            }

            GeminiResponseDto? sugerenciaDto = JsonSerializer.Deserialize<GeminiResponseDto>(textoRespuesta);

            if (sugerenciaDto == null)
            {
                throw new Exception("No se pudo deserializar la respuesta de Gemini.");
            }

            return _mapper.ADominio(sugerenciaDto);
        }

        private GeminiRequestDto CrearRequest(string prompt)
        {
            return new GeminiRequestDto
            {
                Contenidos =
                [
                    new GeminiContenidoDto
            {
                Partes =
                [
                    new GeminiParteDto
                    {
                        Texto = prompt
                    }
                ]
            }
                ],

                GenerationConfig = new GeminiGenerationConfigDto
                {
                    ThinkingConfig = new GeminiThinkingConfigDto
                    {
                        ThinkingBudget = 512
                    }
                }
            };
        }

        private string ConstruirPrompt(List<Insumo> insumosDisponibles, 
                                        Dictionary<int, List<Lote>> vencimientosProximos,
                                        List<string> platosExistentes,
                                        int cantidadPlatos)
        {
            StringBuilder prompt = new();

            prompt.AppendLine("Eres un asistente culinario experto en sugerir platos basados en los ingredientes disponibles y sus fechas de vencimiento próximas. " +
                "Tu tarea es generar sugerencias de platos creativos y deliciosos utilizando los insumos disponibles, " +
                "priorizando aquellos con vencimientos próximos, y evitando sugerir platos que ya existen en la carta del restaurante.");
            prompt.AppendLine();
            
            prompt.AppendLine($"Genera exactamente {cantidadPlatos} platos nuevos.");
            prompt.AppendLine();

            prompt.AppendLine("OBJETIVO:");
            prompt.AppendLine("- Maximizar el aprovechamiento de ingredientes próximos a vencer.");
            prompt.AppendLine("- Utilizar únicamente ingredientes existentes.");
            prompt.AppendLine("- Cada plato debe poder prepararse al menos 5 veces con el stock disponible.");
            prompt.AppendLine("- No sugerir platos ya existentes.");
            prompt.AppendLine();

            prompt.AppendLine("INSUMOS DISPONIBLES:");
            foreach (var insumo in insumosDisponibles)
            {
                prompt.AppendLine($"Id:{insumo.Id} | Nombre:{insumo.Nombre} | Stock:{insumo.StockActual} | Unidad:{insumo.UnidadMedida}");
            }
            prompt.AppendLine();

            prompt.AppendLine("INSUMOS CON VENCIMIENTO PROXIMO:");
            foreach (var item in vencimientosProximos)
            {
                int insumoId = item.Key;
                Insumo? insumo = insumosDisponibles.FirstOrDefault(i => i.Id == insumoId);

                if (insumo == null)
                {
                    continue;
                }
                foreach (var lote in item.Value)
                {
                    prompt.AppendLine($"Id:{insumo.Id} | Nombre:{insumo.Nombre} | Cantidad:{lote.Cantidad} | Vence:{lote.FechaVencimiento:dd/MM/yyyy}");
                }
            }
            prompt.AppendLine();
            
            prompt.AppendLine("PLATOS YA EXISTENTES:");
            foreach (var plato in platosExistentes)
            {
                prompt.AppendLine(plato);
            }
            prompt.AppendLine();

            prompt.AppendLine("REGLAS OBLIGATORIAS:");
            prompt.AppendLine("- No inventar ingredientes.");
            prompt.AppendLine("- No utilizar ingredientes que no estén en la lista.");
            prompt.AppendLine("- Priorizar ingredientes próximos a vencer.");
            prompt.AppendLine("- La cantidad indicada debe corresponder a UNA preparación.");
            prompt.AppendLine("- Para peso utilizar gramos.");
            prompt.AppendLine("- Para líquidos utilizar mililitros.");
            prompt.AppendLine("- Para productos por unidad utilizar unidades.");
            prompt.AppendLine("- El nombre del ingrediente debe coincidir exactamente con el recibido.");
            prompt.AppendLine("- El insumoId debe coincidir exactamente con el recibido.");
            prompt.AppendLine("- No repetir platos.");
            prompt.AppendLine("- No sugerir platos existentes.");
            prompt.AppendLine();

            prompt.AppendLine("RESPONDER EXCLUSIVAMENTE CON JSON VÁLIDO.");
            prompt.AppendLine("NO RESPONDER TEXTO.");
            prompt.AppendLine("NO RESPONDER MARKDOWN.");
            prompt.AppendLine("NO RESPONDER EXPLICACIONES.");
            prompt.AppendLine("- El JSON debe ser válido y deserializable.");
            prompt.AppendLine("- No incluir ```json ni ```.");
            prompt.AppendLine("- No incluir propiedades adicionales.");
            prompt.AppendLine();

            prompt.AppendLine("""
            {
                "platosSugeridos": [
                {
                    "id": 1,
                    "nombre": "string",
                    "descripcion": "string",
                    "tiempoPreparacion": 30,
                    "porcionesPosibles": 5,
                    "ingredientesSugeridosIA": [
                    {
                        "insumoId": 1,
                        "nombre": "string",
                        "cantidad": 500
                    }
                    ]
                }
                ]
            }
            """);

            return prompt.ToString();
        }

        public async Task<PlatoAnalisisIa> AnalizarPlatoRendimientoAsync(
            Plato plato, 
            decimal costoPreparacion, 
            int ventasPeriodo, 
            string volumenVar, 
            string participacion, 
            RendimientoPlato comparativaLider, 
            List<int> tendencia)
        {
            StringBuilder prompt = new();
            prompt.AppendLine("Eres un consultor de negocios culinarios y experto en optimización de menús para restaurantes.");
            prompt.AppendLine("Analiza el siguiente plato que tiene bajo rendimiento de ventas:");
            prompt.AppendLine();
            prompt.AppendLine("PLATO:");
            prompt.AppendLine($"- Nombre: {plato.Nombre}");
            prompt.AppendLine($"- Descripción: {plato.Descripcion}");
            prompt.AppendLine($"- Precio de venta: ${plato.PrecioVentaFinal ?? 0}");
            prompt.AppendLine($"- Costo de preparación: ${costoPreparacion} (Margen de ganancia: ${(plato.PrecioVentaFinal ?? 0) - costoPreparacion})");

            prompt.AppendLine();
            prompt.AppendLine("MÉTRICAS DE RENDIMIENTO (ÚLTIMOS 30 DÍAS):");
            prompt.AppendLine($"- Ventas del periodo: {ventasPeriodo} unidades.");
            prompt.AppendLine($"- Variación: {volumenVar}");
            prompt.AppendLine($"- Participación en su categoría: {participacion}");
            prompt.AppendLine($"- Comparativa con el líder de la categoría ({comparativaLider.Nombre}): El líder vende {comparativaLider.UnidadesVendidas} unidades.");
            prompt.AppendLine($"- Tendencia de ventas semanales (últimas 7 semanas): {string.Join(", ", tendencia)}");
            prompt.AppendLine();
            prompt.AppendLine("TAREA:");
            prompt.AppendLine("1. Genera un Diagnóstico preciso (máximo 2 líneas) de por qué se vende poco (analiza si es por precio alto, bajo margen, popularidad frente al líder, tendencia a la baja o falta de visibilidad).");
            prompt.AppendLine("2. Genera exactamente 2 Sugerencias de acción reales de los siguientes tipos permitidos: 'descuento', 'destacar', 'sugerencia', 'precio', 'receta' con su impacto estimado (Alto, Medio, Bajo) y dificultad (baja, media, alta).");
            prompt.AppendLine();
            prompt.AppendLine("REGLAS OBLIGATORIAS:");
            prompt.AppendLine("- Los tipos de sugerencia válidos son únicamente: 'descuento', 'destacar', 'sugerencia', 'precio', 'receta'.");
            prompt.AppendLine("- Si el diagnóstico indica que el plato tiene baja venta debido a falta de visibilidad o popularidad, debes sugerir el tipo 'destacar' (para que el gerente destaque el plato en el menú).");
            prompt.AppendLine("- Si el plato se ve afectado por un precio de venta alto, debes sugerir el tipo 'descuento', proponiendo una rebaja estratégica que mantenga un buen margen de ganancia.");
            prompt.AppendLine("- Si el plato es relativamente nuevo en la carta (o se infiere que es nuevo por ventas iniciales muy bajas o nulas), debes sugerir el tipo 'sugerencia' (marcarlo como Recomendación del Chef para incentivar a probarlo).");
            prompt.AppendLine("- Responder exclusivamente con JSON válido, sin explicaciones ni formato markdown (no incluir ```json ni ```).");
            prompt.AppendLine();
            prompt.AppendLine("JSON SCHEMA:");
            prompt.AppendLine("""
            {
                "diagnostico": "string",
                "alerta": "critica|media|informativa",
                "sugerencias": [
                {
                    "id": 1,
                    "tipo": "descuento|destacar|sugerencia|precio|receta",
                    "accion": "string con la propuesta concreta",
                    "impacto": "Impacto Alto/Medio/Bajo (+X u./mes)",
                    "dificultad": "baja|media|alta",
                    "esAplicable": true
                }
                ]
            }
            """);

            string apiKey = _configuracion.ApiKey;
            string url = _configuracion.Url;

            GeminiRequestDto requestDto = CrearRequest(prompt.ToString());

            var response = await _httpClient.PostAsJsonAsync($"{url}?key={apiKey}", requestDto);
            response.EnsureSuccessStatusCode();

            string jsonRespuesta = await response.Content.ReadAsStringAsync();
            GeminiApiResponseDto? respuestaGemini = JsonSerializer.Deserialize<GeminiApiResponseDto>(jsonRespuesta);

            if (respuestaGemini == null)
            {
                throw new Exception("No se pudo deserializar la respuesta de Gemini.");
            }

            string textoRespuesta = respuestaGemini?.Candidatos
                                                    .FirstOrDefault()?
                                                    .Contenido
                                                    .Partes
                                                    .FirstOrDefault()?
                                                    .Texto
                                                    ?? throw new Exception("Gemini no devolvió contenido.");

            textoRespuesta = textoRespuesta.Trim();
            if (textoRespuesta.StartsWith("```"))
            {
                int inicio = textoRespuesta.IndexOf('\n') + 1;
                int fin = textoRespuesta.LastIndexOf("```");
                textoRespuesta = textoRespuesta[inicio..fin].Trim();
            }

            PlatoAnalisisGeminiResponseDto? analisisDto = JsonSerializer.Deserialize<PlatoAnalisisGeminiResponseDto>(textoRespuesta);

            if (analisisDto == null)
            {
                throw new Exception("No se pudo deserializar la respuesta de Gemini.");
            }

            return _mapper.ADominio(analisisDto);
        }
    }
}

