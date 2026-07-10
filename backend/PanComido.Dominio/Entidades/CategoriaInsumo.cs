using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class CategoriaInsumo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public TipoInsumo TipoAplica { get; set; }
    }
}
