using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Servicios
{
    public class DisponibilidadArticuloServicio : IDisponibilidadArticuloServicio
    {
        public bool VerificarDisponibilidad(Articulo articulo, Dictionary<int, decimal> stockDeInsumosActual)
        {
            return VerificarDisponibilidad(articulo, 1, stockDeInsumosActual);
        }

        public bool VerificarDisponibilidad(Articulo articulo, int cantidadPedida, Dictionary<int, decimal> stockDeInsumosActual)
        {
            if (articulo is Plato plato)
            {
                return plato.Ingredientes
                    .Where(i => !i.Opcional)
                    .All(i => stockDeInsumosActual.TryGetValue(i.InsumoId, out decimal stock)
                              && stock >= (i.Cantidad * cantidadPedida));
            }

            if (articulo is BebidaPreparada bebidaPreparada)
            {
                return bebidaPreparada.Insumos
                    .All(i => stockDeInsumosActual.TryGetValue(i.InsumoId, out decimal stock)
                              && stock >= (i.Cantidad * cantidadPedida));
            }

            return stockDeInsumosActual.TryGetValue(articulo.Id, out decimal stockDisponible)
                   && stockDisponible >= cantidadPedida;
        }
    }
}
