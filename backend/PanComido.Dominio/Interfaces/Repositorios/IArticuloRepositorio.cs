using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IArticuloRepositorio
    {
        Task<List<Articulo>> ObtenerArticulosEnCartaConIngredientesAsync(int restauranteId);

        Task<Articulo> ObtenerDetalleAsync(int restauranteId, int articuloId);
    }
}
