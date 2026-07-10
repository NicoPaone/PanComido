using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso
{
    public class ObtenerBebidaPreparadaPorIdCasoDeUso
    {
        private readonly IBebidaPreparadaRepositorio _bebidaPreparadaRepositorio;

        public ObtenerBebidaPreparadaPorIdCasoDeUso(IBebidaPreparadaRepositorio bebidaPreparadaRepositorio)
        {
            _bebidaPreparadaRepositorio = bebidaPreparadaRepositorio;
        }

        public async Task<BebidaPreparada> EjecutarAsync(int bebidaPreparadaId, int restauranteId)
        {
            BebidaPreparada bebidaPreparada = await _bebidaPreparadaRepositorio.ObtenerPorIdAsync(bebidaPreparadaId, restauranteId);
            if (bebidaPreparada == null)
            {
                throw new KeyNotFoundException("La bebida preparada no existe o no pertenece al restaurante.");
            }

            return bebidaPreparada;
        }
    }
}
