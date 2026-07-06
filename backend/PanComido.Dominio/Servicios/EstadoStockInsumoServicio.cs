using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Servicios
{
    public class EstadoStockInsumoServicio : IEstadoStockInsumoServicio
    {
        public EstadoStock CalcularEstadoStock(decimal stockActual, decimal stockMinimo, decimal stockRecomendado)
        {
            if (stockActual < stockMinimo) return EstadoStock.Critico;
            else if (stockActual < stockRecomendado) return EstadoStock.Bajo;
            else return EstadoStock.Normal;
        }

    }
}
