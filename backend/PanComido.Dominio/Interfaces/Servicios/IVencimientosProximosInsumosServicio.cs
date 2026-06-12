using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IVencimientosProximosInsumosServicio
    {
        Dictionary<int, List<Lote>> ObtenerVencimientosProximos(
            List<Insumo> insumos,
            int diasAnticipacion);

    }
}
