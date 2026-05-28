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
        public Task<List<Lote>> ObtenerLoteDeInsumoOrdenadoPorVencimientoAscendenteAsync(int insumoId);
        public Task<List<Lote>> ObtenerLoteDeInsumoOrdenadoPorVencimientoDescendenteAsync(int insumoId);

        //public Task<List<Lote>> ObtenerLotesAsync();
        //public Task<Lote> ObtenerLotePorIdAsync(int id);

    }
}
