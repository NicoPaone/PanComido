using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface ILoteRepositorio
    {
        public Task<decimal> ObtenerStockTotalDeInsumo(int insumoId);

        public Task<DateOnly?> ObtenerFechaDeVencimientoMasProximaDeInsumo(int insumoId);

        // Devuelve diccionarios para poder obtener toda la informacion en 1 consulta
        // y no pegarle a la base de datos por cada insumo que tenga las bodegas
        Task<Dictionary<(int insumoId, int bodegaId), decimal>> ObtenerStocksPorBodega(int restauranteId);
        Task<Dictionary<(int insumoId, int bodegaId), DateOnly?>> ObtenerVencimientosPorBodega(int restauranteId);
        Task<Dictionary<int, decimal>> ObtenerStockTotalDeInsumosDisponible(int restauranteId, DateOnly fechaYHora);

        Task<List<Lote>> CrearLotesAsync(List<DOM.Lote> lotes);
        Task<int> ContarLotesConNombreBaseAsync(string nombreBase);
        Task<List<Lote>> ObtenerLotesPorRestauranteAsync(int restauranteId);
        Task<Lote> ObtenerPorIdAsync(int restauranteId, int loteId);
        Task<List<Lote>> ObtenerLotesPorFechaVencimientoAscendenteAsync(int restauranteId, int insumoId);
        Task ActualizarLotesAsync(List<Lote> lotesModificados);
        Task<bool> EliminarAsync(int restauranteId, int loteId);
    }
}