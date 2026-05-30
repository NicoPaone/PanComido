using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class UnidadMedidaEntityMapper
    {
        public DOM.UnidadMedida paraDominio(EF.UnidadMedidum unidadMedidaEF)
        {
            if (unidadMedidaEF == null) return null;

            return new DOM.UnidadMedida
            {
                Id = unidadMedidaEF.Id,
                Nombre = unidadMedidaEF.Nombre,
            };
        }
    }
}
