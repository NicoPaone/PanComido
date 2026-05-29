using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class BodegaEntityMapper
    {
        public DOM.Bodega paraDominio(EF.Bodega bodegaEF)
        {
            if (bodegaEF == null) return null;
            return new DOM.Bodega
            {
                Id = bodegaEF.Id,
                Nombre = bodegaEF.Nombre,
                TipoBodega = bodegaEF.TipoBodega.Descripcion,
                Insumos = new List<DOM.Insumo>()
            };
        }

    }
}
