using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso
{
    public class EliminarReglaTiempoExtraCasoDeUso
    {
        private readonly IReglaTiempoExtraRepositorio _repo;
        public EliminarReglaTiempoExtraCasoDeUso(IReglaTiempoExtraRepositorio repo) => _repo = repo;

        public async Task EjecutarAsync(int id, int restauranteId)
        {
            var regla = await _repo.ObtenerPorIdAsync(id);
            if (regla == null || regla.RestauranteId != restauranteId)
            {
                throw new KeyNotFoundException("Regla no encontrada.");
            }

            await _repo.EliminarAsync(id);
        }
    }
}
