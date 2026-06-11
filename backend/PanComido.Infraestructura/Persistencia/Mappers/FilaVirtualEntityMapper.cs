using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class FilaVirtualEntityMapper
    {
        public DOM.FilaVirtual paraDominio(EF.FilaVirtual efFilaVitaul)
        {
            return new DOM.FilaVirtual
            {
                Id = efFilaVitaul.Id,
                RestauranteId = efFilaVitaul.RestauranteId,
                Habilitada = efFilaVitaul.Habilitada
            };
        }

        public void paraActualizarEntidad(EF.FilaVirtual efFilaExistente, DOM.FilaVirtual filaVirtual)
        {
            efFilaExistente.Habilitada = filaVirtual.Habilitada;
        }
    }
}
