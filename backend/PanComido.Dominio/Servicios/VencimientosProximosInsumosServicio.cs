using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Servicios
{
    public class VencimientosProximosInsumosServicio : IVencimientosProximosInsumosServicio
    {
        public Dictionary<int, List<Lote>> ObtenerVencimientosProximos(List<Insumo> insumos, int diasAnticipacion)
        {
            DateOnly fechaLimite = DateOnly.FromDateTime(DateTime.Now.AddDays(diasAnticipacion));

            Dictionary<int, List<Lote>> vencimientosProximos = new Dictionary<int, List<Lote>>();

            foreach (var insumo in insumos)
            {
                List<Lote> lotesProximos = new();

                foreach (var lote in insumo.Lotes)
                {
                    if (lote.FechaVencimiento <= fechaLimite)
                    {
                        lotesProximos.Add(lote);
                    }
                }
                if (lotesProximos.Any())
                {
                    vencimientosProximos.Add(insumo.Id, lotesProximos);
                }
            }

            return vencimientosProximos;
        }
    }

}
