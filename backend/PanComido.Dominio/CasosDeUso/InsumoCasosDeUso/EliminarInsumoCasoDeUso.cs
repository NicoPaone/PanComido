using System.Collections.Generic;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class EliminarInsumoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;

        public EliminarInsumoCasoDeUso(IInsumoRepositorio insumoRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
        }

        public async Task EjecutarAsync(int insumoId, int restauranteId)
        {
            var eliminado = await _insumoRepositorio.EliminarAsync(insumoId, restauranteId);
            if (eliminado == null)
            {
                throw new KeyNotFoundException("El insumo no existe o no pertenece al restaurante.");
            }
        }
    }
}