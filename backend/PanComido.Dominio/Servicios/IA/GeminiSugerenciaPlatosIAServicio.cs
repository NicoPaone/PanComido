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
        public Task<SugerenciaIA> GenerarSugerenciasAsync(List<Insumo> insumosDisponibles, Dictionary<int, List<Lote>> vencimientosProximos, int cantidadPlatos)
        {
            throw new NotImplementedException();
        }
    }
}
