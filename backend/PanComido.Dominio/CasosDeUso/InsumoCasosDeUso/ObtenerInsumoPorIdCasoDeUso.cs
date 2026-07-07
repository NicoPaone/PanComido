using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class ObtenerInsumoPorIdCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;

        public ObtenerInsumoPorIdCasoDeUso(IInsumoRepositorio insumoRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
        }

        public async Task<Insumo> EjecutarAsync(int insumoId, int restauranteId)
        {
            Insumo insumo = await _insumoRepositorio.ObtenerPorIdAsync(insumoId, restauranteId);
            if (insumo == null)
            {
                throw new KeyNotFoundException("El insumo no existe o no pertenece al restaurante.");
            }
            return insumo;
        }
    }
}