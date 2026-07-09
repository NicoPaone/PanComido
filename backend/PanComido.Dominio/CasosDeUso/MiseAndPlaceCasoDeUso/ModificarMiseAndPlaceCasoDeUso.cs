using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class ModificarMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _repositorio;

        public ModificarMiseAndPlaceCasoDeUso(IMiseAndPlaceRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<bool> EjecutarAsync(int restauranteId, int miseAndPlaceId, ModificarMiseAndPlaceDominio datos)
        {
            var duplicates = datos.Ingredientes.GroupBy(i => i.IngredienteId).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                throw new ArgumentException("Un ingrediente preparado no puede contener el mismo ingrediente más de una vez.");
            }

            return await _repositorio.ModificarMiseAndPlaceAsync(restauranteId, miseAndPlaceId, datos);
        }
    }
}
