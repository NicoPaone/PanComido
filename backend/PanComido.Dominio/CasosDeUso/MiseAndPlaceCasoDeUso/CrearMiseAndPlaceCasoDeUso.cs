using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class CrearMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _miseAndPlaceRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;

        public CrearMiseAndPlaceCasoDeUso(
            IMiseAndPlaceRepositorio miseAndPlaceRepositorio,
            ILoteRepositorio loteRepositorio)
        {
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
            _loteRepositorio = loteRepositorio;
        }

        public async Task<int> EjecutarAsync(NuevoMiseAndPlace nuevoMiseAndPlace)
        {
            string nombreLote = await SugerenciaNombreLote(nuevoMiseAndPlace.Nombre);

            return await _miseAndPlaceRepositorio.CrearMiseAndPlaceAsync(nuevoMiseAndPlace, nombreLote);
        }

        private async Task<string> SugerenciaNombreLote(string nombreInsumo)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string nombreBase = $"{nombreInsumo}-{timestamp}";
            int cantNombreDuplicado = await _loteRepositorio.ContarLotesConNombreBaseAsync(nombreBase);
            if (cantNombreDuplicado == 0) return $"{nombreInsumo}-{timestamp}";
            return $"{nombreInsumo} ({cantNombreDuplicado + 1})-{timestamp}";
        }
    }
}
