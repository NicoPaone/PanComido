using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class MetodoDePagoEntityMapper
    {
        public DOM.MetodoDePago paraDominio(EF.MetodoDePago efMetodoDePago, EF.MetodoDePagoRestaurante efRestaurtante)
        {
            return new DOM.MetodoDePago
            {
               Id = efMetodoDePago.Id,
               Descripcion = efMetodoDePago.Descripcion,
               Habilitado = efRestaurtante.Habilitado
            };
        }

        public EF.MetodoDePagoRestaurante paraActualizarEntidad(DOM.MetodoDePago metodoDePago)
        {
            return new EF.MetodoDePagoRestaurante
            {
                Habilitado = metodoDePago.Habilitado
            };
        }
    }
}
