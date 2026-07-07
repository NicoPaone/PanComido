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
            string nombreLote = await _generadorNombreLoteServicio.GenerarNombreUnicoAsync(nuevoMiseAndPlace.Nombre);

            return await _miseAndPlaceRepositorio.CrearMiseAndPlaceAsync(nuevoMiseAndPlace, nombreLote);
        }
    }
}
