using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IBebidaPreparadaRepositorio
    {
        Task<BebidaPreparada> ObtenerPorIdAsync(int bebidaPreparadaId, int restauranteId);
        Task<bool> ExisteBebidaPreparadaConNombreAsync(int restauranteId, string nombre);
        Task<BebidaPreparada> CrearAsync(BebidaPreparada bebidaPreparada);
        Task<BebidaPreparada> ActualizarAsync(BebidaPreparada bebidaPreparada);
        Task<BebidaPreparada> EliminarAsync(int bebidaPreparadaId, int restauranteId);
        Task<bool> ExisteInsumoEnBebidasActivasAsync(int insumoId);
    }
}
