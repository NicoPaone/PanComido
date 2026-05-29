using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;


namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class CategoriaInsumoEntityMapper
    {
        public DOM.CategoriaInsumo paraDominio(EF.CategoriaInsumo categoriaEF)
        {
            if (categoriaEF == null) return null;
            return new DOM.CategoriaInsumo
            {
                Id = categoriaEF.Id,
                Descripcion = categoriaEF.Descripcion,
                TipoAplica = (TipoInsumo)categoriaEF.TipoAplica
            };
        }
    }
}
