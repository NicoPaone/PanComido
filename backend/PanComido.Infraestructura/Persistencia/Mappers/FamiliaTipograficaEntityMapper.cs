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
    public class FamiliaTipograficaEntityMapper
    {
        public DOM.FamiliaTipografica paraDominio(EF.FamiliaTipografica efFamiliaTipografica)
        {
            return new DOM.FamiliaTipografica
            {
                Id = efFamiliaTipografica.Id,
                Categoria = efFamiliaTipografica.Categoria,
                TipografiaTitulo = efFamiliaTipografica.TipografiaTitulo,
                TipografiaCuerpo = efFamiliaTipografica.TipografiaCuerpo
            };
        }
    }
}
