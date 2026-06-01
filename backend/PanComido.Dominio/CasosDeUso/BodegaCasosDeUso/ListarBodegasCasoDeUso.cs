using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.BodegaCasosDeUso
{
    public class ListarBodegasCasoDeUso
    {
        private readonly IBodegaRepositorio _bodegaRepositorio;

        public ListarBodegasCasoDeUso(IBodegaRepositorio bodegaRepositorio)
        {
            _bodegaRepositorio = bodegaRepositorio;
        }

        public async Task<List<Bodega>> EjecutarAsync(int restauranteId)
        {
            return await _bodegaRepositorio.ObtenerBodegasAsync(restauranteId);
        }

    }
}
