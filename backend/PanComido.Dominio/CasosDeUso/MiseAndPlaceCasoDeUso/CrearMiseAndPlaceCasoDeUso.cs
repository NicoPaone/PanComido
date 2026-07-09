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
        private readonly IGeneradorNombreLoteServicio _generadorNombreLoteServicio;

        public CrearMiseAndPlaceCasoDeUso(
            IMiseAndPlaceRepositorio miseAndPlaceRepositorio,
            IGeneradorNombreLoteServicio generadorNombreLoteServicio)
        {
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
            _generadorNombreLoteServicio = generadorNombreLoteServicio;
        }

        public async Task<int> EjecutarAsync(NuevoMiseAndPlace nuevoMiseAndPlace)
        {
            var duplicates = nuevoMiseAndPlace.Ingredientes.GroupBy(i => i.IngredienteId).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                throw new ArgumentException("Un ingrediente preparado no puede contener el mismo ingrediente más de una vez.");
            }

            string nombreLote = await _generadorNombreLoteServicio.GenerarNombreUnicoAsync(nuevoMiseAndPlace.Nombre);

            return await _miseAndPlaceRepositorio.CrearMiseAndPlaceAsync(nuevoMiseAndPlace, nombreLote);
        }
    }
}
