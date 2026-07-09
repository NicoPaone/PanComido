using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades.Enums;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    internal static class ConfiguracionVisibilidadHelper
    {
        public static async Task AplicarVisibilidadEnCartaAsync(AppDbContext ctx, EF.Articulo efArticulo, bool esVisibleEnCarta)
        {
            var configVisible = await ctx.ConfiguracionArticulos.FindAsync((int)ConfiguracionArticuloEnum.VisibleEnCarta);

            if (esVisibleEnCarta)
            {
                if (!efArticulo.ConfiguracionArticulos.Any(c => c.Id == (int)ConfiguracionArticuloEnum.VisibleEnCarta) && configVisible != null)
                {
                    efArticulo.ConfiguracionArticulos.Add(configVisible);
                }
            }
            else
            {
                var cfg = efArticulo.ConfiguracionArticulos.FirstOrDefault(c => c.Id == (int)ConfiguracionArticuloEnum.VisibleEnCarta);
                if (cfg != null)
                {
                    efArticulo.ConfiguracionArticulos.Remove(cfg);
                }
            }
        }
    }
}
