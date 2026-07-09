using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.BodegaCasosDeUso
{
    public class ModificarBodegaCasoDeUso
    {
        private readonly IBodegaRepositorio _bodegaRepositorio;
        private readonly ITipoBodegaRepositorio _tipoBodegaRepositorio;

        public ModificarBodegaCasoDeUso(IBodegaRepositorio bodegaRepositorio, ITipoBodegaRepositorio tipoBodegaRepositorio)
        {
            _bodegaRepositorio = bodegaRepositorio;
            _tipoBodegaRepositorio = tipoBodegaRepositorio;
        }
        public async Task<Bodega> EjecutarAsync(Bodega bodegaModificada, int restauranteId)
        {
            if (string.IsNullOrWhiteSpace(bodegaModificada.Nombre))
            {
                throw new ArgumentException("El nombre de la bodega no puede estar vacío.");
            }
            if (!await _tipoBodegaRepositorio.ExisteAsync(bodegaModificada.TipoBodegaId))
            {
                throw new ArgumentException("El tipo de bodega proporcionado no es válido.");
            }
            if (await _bodegaRepositorio.ExisteBodegaPorNombreAsync(bodegaModificada.Nombre, restauranteId, bodegaModificada.Id))
            {
                throw new ArgumentException("El nombre ya está siendo utilizado por otra bodega.");
            }
            Bodega bodegaExistente = await _bodegaRepositorio.ObtenerBodegaPorIdAsync(bodegaModificada.Id, restauranteId);
            if (bodegaExistente == null)
            {
                throw new KeyNotFoundException("La bodega que intenta modificar no existe.");
            }
            return await _bodegaRepositorio.ModificarAsync(bodegaModificada, restauranteId);
        }
    }
}
