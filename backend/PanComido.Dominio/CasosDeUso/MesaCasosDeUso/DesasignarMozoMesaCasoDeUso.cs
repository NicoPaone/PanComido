using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class DesasignarMozoMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;

        public DesasignarMozoMesaCasoDeUso(IMesaRepositorio mesaRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
        }

        public async Task EjecutarAsync(int restauranteId, int mesaId, int mozoId)
        {
            await _mesaRepositorio.DesasignarMozoAsync(restauranteId, mesaId, mozoId);
        }
    }
}
