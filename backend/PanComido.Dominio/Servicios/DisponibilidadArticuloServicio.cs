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
            // reutiliza logica asumiendo que se necesita al menos 1 para mostrarlo
            return VerificarDisponibilidad(articulo, 1, stockDeInsumosActual);
        }

        // con cantidad personalizable
        public bool VerificarDisponibilidad(Articulo articulo, int cantidadPedida, Dictionary<int, decimal> stockDeInsumosActual)
        {
            if (articulo is Plato plato)
            {
                return plato.Ingredientes
                    .Where(i => !i.Opcional)
                    .All(i => stockDeInsumosActual.TryGetValue(i.InsumoId, out decimal stock)
                              && stock >= (i.Cantidad * cantidadPedida));
            }

            // Es bebida por descarte
            return stockDeInsumosActual.TryGetValue(articulo.Id, out decimal stockDisponible)
                   && stockDisponible >= cantidadPedida;
        }
    }
}
