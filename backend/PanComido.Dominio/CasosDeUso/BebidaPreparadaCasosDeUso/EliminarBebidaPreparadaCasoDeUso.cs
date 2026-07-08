using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso
{
    public class EliminarBebidaPreparadaCasoDeUso
    {
        private readonly IBebidaPreparadaRepositorio _bebidaPreparadaRepositorio;

        public EliminarBebidaPreparadaCasoDeUso(IBebidaPreparadaRepositorio bebidaPreparadaRepositorio)
        {
            _bebidaPreparadaRepositorio = bebidaPreparadaRepositorio;
        }

        public async Task EjecutarAsync(int bebidaPreparadaId, int restauranteId)
        {
            var eliminada = await _bebidaPreparadaRepositorio.EliminarAsync(bebidaPreparadaId, restauranteId);
            if (eliminada == null)
            {
                throw new KeyNotFoundException("La bebida preparada no existe o no pertenece al restaurante.");
            }
        }
    }
}
