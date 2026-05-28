using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Infraestructura.Persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class InsumoEntityMapper
    {
        public DOM.Insumo paraDominio(EF.Articulo efArticulo)
        {
            if (efArticulo == null) return null;

            // El scaffold genera: efArticulo.Insumo
            EF.Insumo efInsumo = efArticulo.Insumo
                ?? throw new InvalidOperationException("Articulo no es un insumo");
            
            // Detectar subtipo
            bool esIngrediente = efInsumo.Ingrediente != null;
            bool esBebida = efInsumo.Bebidum != null;

            return new DOM.Insumo
            {
                Id = efArticulo.Id,
                Nombre = efArticulo.Nombre,
                Descripcion = efArticulo.Descripcion,
                StockMinimo = efInsumo.StockMinimo,
                Tipo = esIngrediente ? TipoInsumo.Ingrediente
                                              : TipoInsumo.Bebida,
                Categoria = esIngrediente
                    ? efInsumo.Ingrediente
                              .CategoriaIngrediente?.Descripcion
                    : efInsumo.Bebidum
                              .CategoriaBebida?.Descripcion,
                
                UnidadMedida = esIngrediente
                    ? efInsumo.Ingrediente
                              .UnidadMedida?.Nombre
                    : null,
            };

        }
    }
}
