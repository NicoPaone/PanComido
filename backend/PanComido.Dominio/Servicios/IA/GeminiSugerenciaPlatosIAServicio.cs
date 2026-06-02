using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Servicios.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Servicios.IA
{
    public class GeminiSugerenciaPlatosIAServicio : ISugerenciaPlatosIAServicio
    {
        private readonly HttpClient _httpClient;

        public GeminiSugerenciaPlatosIAServicio(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SugerenciaIA> GenerarSugerenciasAsync(List<Insumo> insumosDisponibles, 
                                                                Dictionary<int, List<Lote>> vencimientosProximos, 
                                                                int cantidadPlatos)
        {
            string prompt = ConstruirPrompt(insumosDisponibles,
                                            vencimientosProximos,
                                            cantidadPlatos);
            
            throw new NotImplementedException();
        }

        private string ConstruirPrompt(List<Insumo> insumosDisponibles, 
                                        Dictionary<int, List<Lote>> vencimientosProximos, 
                                        int cantidadPlatos)
        {
            return null;
        }
    }
}
