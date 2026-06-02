using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IFormularioPlatoRepositorio
    {
        Task <List <TipoPlato>> ObtenerTiposPlatoAsync();
        Task<List<CategoriaPlato>> ObtenerCategoriasPlatoAsync();
        Task<List<Restriccion>> ObtenerRestriccionesAsync();
        Task<List<Ingrediente>> ObtenerIngredientesBaseAsync(int restauranteId);
        Task<List<IngredientePreparado>> ObtenerIngredientesPreparadosAsync(int restauranteId);
    }
}
