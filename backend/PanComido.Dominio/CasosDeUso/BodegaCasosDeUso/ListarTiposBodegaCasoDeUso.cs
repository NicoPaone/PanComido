using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.BodegaCasosDeUso
{
    public class ListarTiposBodegaCasoDeUso
    {
        private readonly ITipoBodegaRepositorio _repo;
        
        public ListarTiposBodegaCasoDeUso(ITipoBodegaRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<List<TipoBodega>> EjecutarAsync()
        {
            return await _repo.ObtenerTodosAsync();
        }
    }
}
