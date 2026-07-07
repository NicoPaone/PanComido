using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso
{
    public class ObtenerReglasTiempoExtraCasoDeUso
    {
        private readonly IReglaTiempoExtraRepositorio _repo;
        public ObtenerReglasTiempoExtraCasoDeUso(IReglaTiempoExtraRepositorio repo) => _repo = repo;

        public async Task<List<ReglaTiempoExtra>> EjecutarAsync(int restauranteId)
            => await _repo.ObtenerPorRestauranteIdAsync(restauranteId);
    }
}
