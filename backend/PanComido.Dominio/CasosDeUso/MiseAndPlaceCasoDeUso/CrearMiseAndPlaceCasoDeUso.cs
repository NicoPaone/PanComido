using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class CrearMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _miseAndPlaceRepositorio;
        private readonly IInsumoValidacionServicio _insumoValidacionServicio;
        private readonly IGeneradorNombreLoteServicio _generadorNombreLoteServicio;

        public CrearMiseAndPlaceCasoDeUso(
            IMiseAndPlaceRepositorio miseAndPlaceRepositorio,
            IInsumoValidacionServicio insumoValidacionServicio,
            IGeneradorNombreLoteServicio generadorNombreLoteServicio)
        {
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
            _insumoValidacionServicio = insumoValidacionServicio;
            _generadorNombreLoteServicio = generadorNombreLoteServicio;
        }

        public async Task<int> EjecutarAsync(NuevoMiseAndPlace nuevoMiseAndPlace)
        {
            var duplicates = nuevoMiseAndPlace.Ingredientes.GroupBy(i => i.IngredienteId).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                throw new ArgumentException("Un ingrediente preparado no puede contener el mismo ingrediente más de una vez.");
            }

            if (nuevoMiseAndPlace.Ingredientes != null && nuevoMiseAndPlace.Ingredientes.Any())
            {
                var insumoIds = nuevoMiseAndPlace.Ingredientes.Select(i => i.IngredienteId).ToList();
                await _insumoValidacionServicio.ValidarInsumosActivosAsync(insumoIds, nuevoMiseAndPlace.RestauranteId);
            }

            string nombreLote = await _generadorNombreLoteServicio.GenerarNombreUnicoAsync(nuevoMiseAndPlace.Nombre);

            return await _miseAndPlaceRepositorio.CrearMiseAndPlaceAsync(nuevoMiseAndPlace, nombreLote);
        }
    }
}
