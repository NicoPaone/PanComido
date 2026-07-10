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
        private readonly IInsumoRepositorio _insumoRepositorio;

        public CrearMiseAndPlaceCasoDeUso(
            IMiseAndPlaceRepositorio miseAndPlaceRepositorio,
            IInsumoValidacionServicio insumoValidacionServicio,
            IInsumoRepositorio insumoRepositorio)
        {
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
            _insumoValidacionServicio = insumoValidacionServicio;
            _insumoRepositorio = insumoRepositorio;
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

            bool existeNombre = await _insumoRepositorio.ExisteInsumoConNombreAsync(nuevoMiseAndPlace.RestauranteId, nuevoMiseAndPlace.Nombre);
            if (existeNombre)
            {
                throw new ArgumentException("Ese nombre ya existe. Elija otro nombre");
            }

            return await _miseAndPlaceRepositorio.CrearMiseAndPlaceAsync(nuevoMiseAndPlace);
        }
    }
}
