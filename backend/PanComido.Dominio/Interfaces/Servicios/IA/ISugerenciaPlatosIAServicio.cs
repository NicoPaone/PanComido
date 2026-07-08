using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios.IA
{
    public interface ISugerenciaPlatosIAServicio
    {
        Task<SugerenciaIA> GenerarSugerenciasAsync(int restauranteId,
                                                    List<Insumo> insumosDisponibles,
                                                    Dictionary<int, List<Lote>> vencimientosProximos,
                                                    List<string> platosExistentes,
                                                    int cantidadPlatos);

        Task<PlatoAnalisisIa> AnalizarPlatoRendimientoAsync(
            Plato plato, 
            decimal costoPreparacion, 
            int ventasPeriodo, 
            string volumenVar, 
            string participacion, 
            RendimientoPlato comparativaLider, 
            List<int> tendencia);
    }
}
