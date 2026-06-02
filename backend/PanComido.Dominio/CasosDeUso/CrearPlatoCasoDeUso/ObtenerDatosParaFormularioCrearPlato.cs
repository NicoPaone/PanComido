using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso
{
    public class ObtenerDatosParaFormularioCrearPlato
    {
        private readonly IFormularioPlatoRepositorio _repositorio;

        public ObtenerDatosParaFormularioCrearPlato(IFormularioPlatoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task < DatosFormularioCrearPlato> Ejecutar (int restauranteId)
        {
            var datos = new DatosFormularioCrearPlato
            {
                TiposPlato = await _repositorio.ObtenerTiposPlatoAsync(),
                CategoriasPlato = await _repositorio.ObtenerCategoriasPlatoAsync(),
                Restricciones = await _repositorio.ObtenerRestriccionesAsync(),
                Ingredientes = await _repositorio.ObtenerIngredientesBaseAsync(restauranteId),
                IngredientePreparados = await _repositorio.ObtenerIngredientesPreparadosAsync(restauranteId)
            };
            return datos;
        }

    }
}
