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
    public class PlatoEntityMapper
    {
        public DOM.Plato CompletarMapeoDominio(EF.Articulo efArticulo)
        {
            return new DOM.Plato
            {
                TiempoPreparacionBase = efArticulo.Plato.TiempoPreparacionBase,
                Destacado = efArticulo.Plato.Destacado,
                Sugerencia = efArticulo.Plato.Sugerencia,
                CategoriaPlatoId = efArticulo.Plato.CategoriaPlatoId,
                TipoPlatoId = efArticulo.Plato.TipoPlatoId,

                Ingredientes = efArticulo.Plato.PlatoIngredientes?.Select(pi => new DOM.PlatoIngrediente
                {
                    InsumoId = pi.IngredienteId,
                    Opcional = pi.Opcional,
                    Cantidad = pi.Cantidad
                }).ToList() ?? new List<DOM.PlatoIngrediente>()
            };
        }

        public EF.Plato CompletarMapeoAEntidad(DOM.Plato platoDominio)
        {
            return new EF.Plato
            {
                TiempoPreparacionBase = platoDominio.TiempoPreparacionBase,
                Destacado = platoDominio.Destacado,
                Sugerencia = platoDominio.Sugerencia,
                CategoriaPlatoId = platoDominio.CategoriaPlatoId,
                TipoPlatoId = platoDominio.TipoPlatoId,

                PlatoIngredientes = platoDominio.Ingredientes?.Select(i => new EF.PlatoIngrediente
                {
                    IngredienteId = i.InsumoId,
                    Opcional = i.Opcional,
                    Cantidad = i.Cantidad
                }).ToList() ?? new List<EF.PlatoIngrediente>()
            };
        }
    }
}
