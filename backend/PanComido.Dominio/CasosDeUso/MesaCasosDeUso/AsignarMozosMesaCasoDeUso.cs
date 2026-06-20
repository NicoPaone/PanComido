using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class AsignarMozosMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;

        public AsignarMozosMesaCasoDeUso(IMesaRepositorio mesaRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
        }

        public async Task EjecutarAsync(int restauranteId, int mesaId, List<int> mozosIds)
        {
            await _mesaRepositorio.AsignarMozosAsync(restauranteId, mesaId, mozosIds);
        }
    }
}
