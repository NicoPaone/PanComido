using Microsoft.EntityFrameworkCore;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using PanComido.Dominio.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class RestauranteRepositorio : IRestauranteRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly RestauranteEntityMapper _restauranteMapper;

        public RestauranteRepositorio(AppDbContext context, RestauranteEntityMapper restauranteMapper)
        {
            _ctx = context;
            _restauranteMapper = restauranteMapper;
        }

        public async Task<DOM.Restaurante?> ObtenerDatosDelLocalAsync(int restauranteId)
        {
            var efRestaurante = await _ctx.Restaurantes
                .Include(r => r.Direccion)
                .Include(r => r.FamiliaTipografica)
                .FirstOrDefaultAsync(r => r.Id == restauranteId);

            return efRestaurante == null ? null : _restauranteMapper.paraDominio(efRestaurante);
        }

        public async Task ActualizarDatosDelLocalAsync(int restauranteId, Restaurante datosActualizados)
        {
            var efRestaurante = await _ctx.Restaurantes
                .Include(r => r.Direccion)
                .FirstOrDefaultAsync(r => r.Id == restauranteId);

            if (efRestaurante == null) return;

            _restauranteMapper.paraActualizarEntidad(efRestaurante, datosActualizados);

            await _ctx.SaveChangesAsync();
        }
    }
}