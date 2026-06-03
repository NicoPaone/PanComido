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
            if (loteDom == null) return null;

            return new EF.Lote
            {
                Id = loteDom.Id,
                InsumoId = loteDom.InsumoId,
                BodegaId = loteDom.BodegaId,
                Nombre = loteDom.Nombre,
                Cantidad = loteDom.Cantidad,
                FechaAdquisicion = loteDom.FechaAdquisicion,
                FechaVencimiento = loteDom.FechaVencimiento
            };
        }

        public DOM.Lote paraDominio(EF.Lote loteEF)
        {
            if (loteEF == null) return null;

            return new DOM.Lote
            {
                Id = loteEF.Id,
                InsumoId = loteEF.InsumoId,
                BodegaId = loteEF.BodegaId,
                Nombre = loteEF.Nombre,
                Cantidad = loteEF.Cantidad,
                FechaAdquisicion = loteEF.FechaAdquisicion,

                FechaVencimiento = loteEF.FechaVencimiento.GetValueOrDefault()
            };
        }
    }
}
