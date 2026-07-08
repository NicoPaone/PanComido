using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IEstadoStockInsumoServicio
    {
        EstadoStock CalcularEstadoStock(decimal stockActual, decimal stockMinimo, decimal stockRecomendado);

    }
}
