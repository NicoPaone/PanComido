using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class CrearMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _miseAndPlaceRepositorio;
        private readonly IGeneradorNombreLoteServicio _generadorNombreLoteServicio;
        private readonly IGestionStockServicio _gestionStockServicio;
        private readonly IInsumoRepositorio _insumoRepositorio;

        public CrearMiseAndPlaceCasoDeUso(
            IMiseAndPlaceRepositorio miseAndPlaceRepositorio,
            IGeneradorNombreLoteServicio generadorNombreLoteServicio,
            IGestionStockServicio gestionStockServicio,
            IInsumoRepositorio insumoRepositorio)
        {
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
            _generadorNombreLoteServicio = generadorNombreLoteServicio;
            _gestionStockServicio = gestionStockServicio;
            _insumoRepositorio = insumoRepositorio;
        }

        public async Task<int> EjecutarAsync(NuevoMiseAndPlace nuevoMiseAndPlace)
        {
            var duplicates = nuevoMiseAndPlace.Ingredientes.GroupBy(i => i.IngredienteId).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                throw new ArgumentException("Un ingrediente preparado no puede contener el mismo ingrediente más de una vez.");
            }

            bool existeNombre = await _insumoRepositorio.ExisteInsumoConNombreAsync(nuevoMiseAndPlace.RestauranteId, nuevoMiseAndPlace.Nombre);
            if (existeNombre)
            {
                throw new ArgumentException("Ese nombre ya existe. Elija otro nombre");
            }

            if (nuevoMiseAndPlace.RendimientoBase <= 0)
            {
                throw new ArgumentException("El rendimiento base debe ser mayor a cero.");
            }

            if (nuevoMiseAndPlace.Cantidad <= 0)
            {
                throw new ArgumentException("La cantidad inicial a producir debe ser mayor a cero.");
            }

            var insumosARestar = new Dictionary<int, decimal>();
            decimal factorMultiplicador = nuevoMiseAndPlace.Cantidad / nuevoMiseAndPlace.RendimientoBase;

            foreach (var ingrediente in nuevoMiseAndPlace.Ingredientes)
            {
                bool insumoValido = await _insumoRepositorio.ExisteInsumoAsync(ingrediente.IngredienteId, nuevoMiseAndPlace.RestauranteId);
                if (!insumoValido)
                {
                    throw new ArgumentException($"El ingrediente con ID {ingrediente.IngredienteId} no existe o no pertenece a su restaurante.");
                }

                decimal cantidadADescontar = ingrediente.Cantidad * factorMultiplicador;
                insumosARestar[ingrediente.IngredienteId] = cantidadADescontar;
            }

            await _gestionStockServicio.DescontarStockInsumosAsync(nuevoMiseAndPlace.RestauranteId, insumosARestar);

            string nombreLote = await _generadorNombreLoteServicio.GenerarNombreUnicoAsync(nuevoMiseAndPlace.Nombre);

            return await _miseAndPlaceRepositorio.CrearMiseAndPlaceAsync(nuevoMiseAndPlace, nombreLote);
        }
    }
}
