using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class ProducirMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _miseAndPlaceRepositorio;
        private readonly IGestionStockServicio _gestionStockServicio;
        private readonly IGeneradorNombreLoteServicio _generadorNombreLoteServicio;

        public ProducirMiseAndPlaceCasoDeUso(
            IMiseAndPlaceRepositorio miseAndPlaceRepositorio,
            IGestionStockServicio gestionStockServicio,
            IGeneradorNombreLoteServicio generadorNombreLoteServicio)
        {
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
            _gestionStockServicio = gestionStockServicio;
            _generadorNombreLoteServicio = generadorNombreLoteServicio;
        }

        public async Task<int> EjecutarAsync(int restauranteId, int miseAndPlaceId, decimal cantidadAProducir, DateOnly fechaVencimiento, int bodegaId)
        {
            if (cantidadAProducir <= 0)
            {
                throw new ArgumentException("La cantidad a producir debe ser mayor a cero.");
            }

            var miseAndPlace = await _miseAndPlaceRepositorio.ObtenerPorIdAsync(restauranteId, miseAndPlaceId);
            if (miseAndPlace == null)
            {
                throw new ArgumentException("Mise And Place no encontrado.");
            }

            if (miseAndPlace.RendimientoBase <= 0)
            {
                throw new InvalidOperationException("El rendimiento base de la receta no es válido.");
            }

            var insumosARestar = new Dictionary<int, decimal>();
            decimal factorMultiplicador = cantidadAProducir / miseAndPlace.RendimientoBase;

            foreach (var ingrediente in miseAndPlace.Receta)
            {
                decimal cantidadADescontar = ingrediente.Cantidad * factorMultiplicador;
                insumosARestar[ingrediente.IngredienteId] = cantidadADescontar;
            }

            // Descontar stock (validará que haya suficiente y aplicará FIFO)
            await _gestionStockServicio.DescontarStockInsumosAsync(restauranteId, insumosARestar);

            // Generar nombre de lote único
            string nombreLote = await _generadorNombreLoteServicio.GenerarNombreUnicoAsync(miseAndPlace.Nombre);

            // Crear el lote físico
            return await _miseAndPlaceRepositorio.ProducirMiseAndPlaceAsync(restauranteId, miseAndPlaceId, cantidadAProducir, fechaVencimiento, bodegaId, nombreLote);
        }
    }
}
