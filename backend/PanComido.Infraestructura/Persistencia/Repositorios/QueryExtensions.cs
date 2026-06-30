using PanComido.Dominio.Entidades;
using PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public static class QueryExtensions
    {
        public static IQueryable<ArticuloComandum> FiltrarPagadasEnRango(
            this IQueryable<ArticuloComandum> query, int restauranteId, DateTime desde, DateTime hasta)
        {
            return query.Where(ac =>
                ac.Comanda.RestauranteId == restauranteId &&
                ac.Comanda.HoraInicio >= desde &&
                ac.Comanda.HoraInicio <= hasta &&
                ac.Comanda.Pagos.Any());
        }
    }
}
