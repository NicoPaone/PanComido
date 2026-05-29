using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface ILoteRepositorio
    {
        public Task<decimal> ObtenerStockTotalDeInsumo(int insumoId);

        public Task<DateOnly?> ObtenerFechaDeVencimientoMasProximaDeInsumo(int insumoId);

        // Devuelve diccionarios para poder obtener toda la informacion en 1 consulta
        // y no pegarle a la base de datos por cada insumo que tenga las bodegas
        Task<Dictionary<(int insumoId, int bodegaId), decimal>> ObtenerStocksPorBodega(int restauranteId);
        Task<Dictionary<(int insumoId, int bodegaId), DateOnly?>> ObtenerVencimientosPorBodega(int restauranteId);

    }
}
