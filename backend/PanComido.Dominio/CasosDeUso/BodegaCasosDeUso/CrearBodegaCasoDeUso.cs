using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.BodegaCasosDeUso
{
    public class CrearBodegaCasoDeUso
    {
        private readonly IBodegaRepositorio _bodegaRepositorio;
        private readonly ITipoBodegaRepositorio _tipoBodegaRepositorio;

        public CrearBodegaCasoDeUso(IBodegaRepositorio bodegaRepositorio, ITipoBodegaRepositorio tipoBodegaRepositorio)
        {
            _bodegaRepositorio = bodegaRepositorio;
            _tipoBodegaRepositorio = tipoBodegaRepositorio;
        }
        public async Task<Bodega> EjecutarAsync(Bodega bodega, int restauranteId)
        {
            if (string.IsNullOrWhiteSpace(bodega.Nombre))
            {
                throw new ArgumentException("El nombre de la bodega no puede estar vacío.");
            }
            if (!await _tipoBodegaRepositorio.ExisteAsync(bodega.TipoBodegaId))
            {
                throw new ArgumentException("El tipo de bodega proporcionado no es válido.");
            }
            return await _bodegaRepositorio.CrearAsync(bodega, restauranteId);
        }
    }
}
