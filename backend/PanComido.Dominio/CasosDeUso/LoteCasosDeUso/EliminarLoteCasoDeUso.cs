using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.LoteCasosDeUso
{
    public class EliminarLoteCasoDeUso
    {
        private readonly ILoteRepositorio _loteRepositorio;

        public EliminarLoteCasoDeUso(ILoteRepositorio loteRepositorio)
        {
            _loteRepositorio = loteRepositorio;
        }

        public async Task<bool> EjecutarAsync(int restauranteId, int loteId)
        {
            var lote = await _loteRepositorio.ObtenerPorIdAsync(restauranteId, loteId);
            if (lote == null)
            {
                return false;
            }

            if (lote.Cantidad > 0)
            {
                throw new InvalidOperationException("No se puede eliminar el lote porque todavía tiene stock disponible. Debe registrar su consumo o pérdida (cantidad = 0) antes de eliminarlo.");
            }

            return await _loteRepositorio.EliminarAsync(restauranteId, loteId);
        }
    }
}
