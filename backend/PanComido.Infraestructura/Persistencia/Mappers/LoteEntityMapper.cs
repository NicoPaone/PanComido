using EF = PanComido.Infraestructura.Persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class LoteEntityMapper
    {
        public EF.Lote paraEntidad(DOM.Lote loteDom)
        {
            return new EF.Lote 
            { 
            InsumoId = loteDom.InsumoId,
            BodegaId = loteDom.BodegaId,
            Nombre = loteDom.Nombre,
            Cantidad = loteDom.Cantidad,
            FechaAdquisicion = loteDom.FechaAdquisicion,
            FechaVencimiento = loteDom.FechaVencimiento
            };
        }
    }
}
