using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Comanda
{
    public class ListarComandaActivaCocinaCasoDeUso
    {
        public readonly IComandaRepositorio _comandaRepositorio;

        public ListarComandaActivaCocinaCasoDeUso(IComandaRepositorio comandaRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
        }

        public async Task<List<Entidades.Comanda>> Ejecutar(int restauranteId)
        {
            return await _comandaRepositorio.ObtenerComandasActivasAsync(restauranteId);
        }


    }
}
