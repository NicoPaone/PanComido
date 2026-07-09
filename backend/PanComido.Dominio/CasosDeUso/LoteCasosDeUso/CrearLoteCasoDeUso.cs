using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.LoteCasosDeUso
{
    public class CrearLoteCasoDeUso
    {
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IGeneradorNombreLoteServicio _generadorNombreLoteServicio;

        public CrearLoteCasoDeUso(
            ILoteRepositorio loteRepositorio,
            IInsumoRepositorio insumoRepositorio,
            IGeneradorNombreLoteServicio generadorNombreLoteServicio)
        {
            _loteRepositorio = loteRepositorio;
            _insumoRepositorio = insumoRepositorio;
            _generadorNombreLoteServicio = generadorNombreLoteServicio;
        }

        public async Task<int> EjecutarAsync(int restauranteId, int insumoId, decimal cantidad, DateOnly fechaVencimiento, int bodegaId)
        {
            var insumo = await _insumoRepositorio.ObtenerPorIdAsync(insumoId, restauranteId);
            if (insumo == null)
            {
                throw new Exception("El insumo especificado no existe.");
            }

            string nombreLote = await _generadorNombreLoteServicio.GenerarNombreUnicoAsync(insumo.Nombre);

            var nuevoLote = new Lote
            {
                Nombre = nombreLote,
                InsumoId = insumoId,
                BodegaId = bodegaId,
                Cantidad = cantidad,
                FechaAdquisicion = DateOnly.FromDateTime(DateTime.UtcNow),
                FechaVencimiento = fechaVencimiento
            };

            await _loteRepositorio.CrearLotesAsync(new List<Lote> { nuevoLote });

            // Since CrearLotesAsync doesn't return the ID, and we don't strictly need it right now for the response, 
            // we can just return a success indicator or 0. If we need the ID, ILoteRepositorio would need an update.
            return 1;
        }
    }
}
