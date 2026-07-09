using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.BodegaCasosDeUso
{
    public class EliminarBodegaCasoDeUso
    {
        private readonly IBodegaRepositorio _bodegaRepositorio;
        public EliminarBodegaCasoDeUso(IBodegaRepositorio bodegaRepositorio)
        {
            _bodegaRepositorio = bodegaRepositorio;
        }
        public async Task EjecutarAsync(int id, int restauranteId)
        {
            Bodega bodegaExistente = await _bodegaRepositorio.ObtenerBodegaPorIdAsync(id, restauranteId);
            if (bodegaExistente == null)
            {
                throw new KeyNotFoundException("La bodega que intenta eliminar no existe.");
            }
            bool tieneLotes = await _bodegaRepositorio.TieneLotesAsociadosAsync(id);
            if (tieneLotes)
            {
                throw new InvalidOperationException("No se puede eliminar la bodega porque contiene lotes físicos (mercadería) asociados.");
            }
            await _bodegaRepositorio.EliminarAsync(id, restauranteId);
        }
    }
}
