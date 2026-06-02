using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ComandaCasosDeUso
{
    public class ListarComandasActivasMozoCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;

        public ListarComandasActivasMozoCasoDeUso(IComandaRepositorio comandaRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
        }

        public async Task<List<Entidades.Comanda>> EjecutarAsync(int restauranteId, int mozoId)
        {
            return await _comandaRepositorio.ObtenerComandasActivasPorMozoAsync(restauranteId, mozoId);
        }
    }
}
