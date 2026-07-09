using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.LoteCasosDeUso
{
    public class ModificarLoteCasoDeUso
    {
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IGeneradorNombreLoteServicio _generadorNombreLoteServicio;

        public ModificarLoteCasoDeUso(
            ILoteRepositorio loteRepositorio,
            IInsumoRepositorio insumoRepositorio,
            IGeneradorNombreLoteServicio generadorNombreLoteServicio)
        {
            _loteRepositorio = loteRepositorio;
            _insumoRepositorio = insumoRepositorio;
            _generadorNombreLoteServicio = generadorNombreLoteServicio;
        }

        public async Task<bool> EjecutarAsync(int restauranteId, int loteId, int insumoId, decimal cantidad, DateOnly fechaVencimiento, int bodegaId)
        {
            var lote = await _loteRepositorio.ObtenerPorIdAsync(restauranteId, loteId);
            if (lote == null)
            {
                return false;
            }

            if (lote.InsumoId != insumoId)
            {
                var nuevoInsumo = await _insumoRepositorio.ObtenerPorIdAsync(insumoId, restauranteId);
                if (nuevoInsumo == null)
                {
                    throw new Exception("El nuevo insumo especificado no existe.");
                }

                lote.Nombre = await _generadorNombreLoteServicio.GenerarNombreUnicoAsync(nuevoInsumo.Nombre);
                lote.InsumoId = insumoId;
            }

            lote.Cantidad = cantidad;
            lote.FechaVencimiento = fechaVencimiento;
            lote.BodegaId = bodegaId;

            await _loteRepositorio.ActualizarLotesAsync(new List<Lote> { lote });

            return true;
        }
    }
}
