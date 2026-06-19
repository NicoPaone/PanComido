using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Servicios
{
    public class CalcularTotalComandaServicio : ICalcularTotalComandaServicio
    {
        public decimal CalcularTotal(Comanda comanda)
        {
            decimal total = 0;
            foreach (var item in comanda.Items)
            {
                total += item.Cantidad * (item.Articulo.PrecioVentaFinal ?? 0);
            }
            return total;
        }
    }
}
