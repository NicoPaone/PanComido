using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Linq;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class ModificarMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _repositorio;
        private readonly IInsumoValidacionServicio _insumoValidacionServicio;

        public ModificarMiseAndPlaceCasoDeUso(IMiseAndPlaceRepositorio repositorio, IInsumoValidacionServicio insumoValidacionServicio)
        {
            _repositorio = repositorio;
            _insumoValidacionServicio = insumoValidacionServicio;
        }

        public async Task<bool> EjecutarAsync(int restauranteId, int miseAndPlaceId, ModificarMiseAndPlaceDominio datos)
        {
            var duplicates = datos.Ingredientes.GroupBy(i => i.IngredienteId).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                throw new ArgumentException("Un ingrediente preparado no puede contener el mismo ingrediente más de una vez.");
            }

            if (datos.Ingredientes != null && datos.Ingredientes.Any())
            {
                var insumoIds = datos.Ingredientes.Select(i => i.IngredienteId).ToList();
                await _insumoValidacionServicio.ValidarInsumosActivosAsync(insumoIds, restauranteId);
            }

            return await _repositorio.ModificarMiseAndPlaceAsync(restauranteId, miseAndPlaceId, datos);
        }
    }
}
