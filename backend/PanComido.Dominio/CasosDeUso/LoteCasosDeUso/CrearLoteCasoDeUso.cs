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

        public async Task<Lote> EjecutarAsync(int restauranteId, int insumoId, decimal cantidad, DateOnly fechaVencimiento, int bodegaId)
        {
            var insumo = await _insumoRepositorio.ObtenerPorIdAsync(insumoId, restauranteId);
            if (insumo == null) throw new KeyNotFoundException("El insumo especificado no existe.");

            if(fechaVencimiento < DateOnly.FromDateTime(DateTime.Today)) throw new ArgumentException("La fecha de vencimiento debe ser posterior a la de hoy");

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

            var lotesCreados =  await _loteRepositorio.CrearLotesAsync(new List<Lote> { nuevoLote });

            return lotesCreados.First();
        }
    }
}
