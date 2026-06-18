using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class DatosFormularioCrearPlato
    {
        public List <TipoPlato> TiposPlato { get; set; } = new List<TipoPlato>() ;
        public List <CategoriaPlato> CategoriasPlato { get; set; } = new List<CategoriaPlato>();
        public List<Restriccion> Restricciones { get; set; } = new List<Restriccion>();
        public List<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
        public List<IngredientePreparado> IngredientePreparados { get; set; } = new List<IngredientePreparado>();
        public PorcentajesGanancia Porcentajes { get; set; }

    }


}
