using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface ICalcularTotalComandaServicio
    {
        decimal CalcularTotal(Comanda comanda);
    }
}
