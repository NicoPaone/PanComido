using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IPlatoAnalisisRepositorio
    {
        Task<Articulo?> ObtenerArticuloConPlatoYIngredientesPorNombreAsync(int restauranteId, string nombre);
        Task<decimal> ObtenerUltimoPrecioCompraInsumoAsync(int insumoId);
        Task<int> ObtenerVentasArticuloEnRangoAsync(int restauranteId, int articuloId, DateTime desde, DateTime hasta);
        Task<int> ObtenerVentasCategoriaEnRangoAsync(int restauranteId, int categoriaPlatoId, DateTime desde, DateTime hasta);
        Task<RendimientoPlato?> ObtenerPlatoLiderDeCategoriaAsync(int restauranteId, int categoriaPlatoId, DateTime desde, DateTime hasta);
        Task GuardarRecordatorioNotificacionAsync(int restauranteId, string descripcion);
    }
}
