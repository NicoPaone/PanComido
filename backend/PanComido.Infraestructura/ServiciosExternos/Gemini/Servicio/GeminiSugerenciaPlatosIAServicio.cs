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

            Console.WriteLine($"URL: '{url}'");
            Console.WriteLine($"API KEY: '{apiKey}'");

            GeminiRequestDto requestDto = CrearRequest(prompt);

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
                ]
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
    }
}
