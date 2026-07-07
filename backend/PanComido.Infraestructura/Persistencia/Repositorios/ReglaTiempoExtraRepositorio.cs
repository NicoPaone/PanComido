using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class ReglaTiempoExtraRepositorio : IReglaTiempoExtraRepositorio
    {
        private readonly AppDbContext _context;
        private readonly ReglaTiempoExtraEntityMapper _mapper;

        public ReglaTiempoExtraRepositorio(AppDbContext context, ReglaTiempoExtraEntityMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DOM.ReglaTiempoExtra> ObtenerPorIdAsync(int id)
        {
            var entidad = await _context.ReglaTiempoExtras.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            return _mapper.aDominio(entidad);
        }

        public async Task<List<DOM.ReglaTiempoExtra>> ObtenerPorRestauranteIdAsync(int restauranteId)
        {
            var entidades = await _context.ReglaTiempoExtras
                .AsNoTracking()
                .Where(r => r.RestauranteId == restauranteId)
                .OrderBy(r => r.PorcentajeOcupacionHasta)
                .ToListAsync();

            return _mapper.aDominios(entidades);
        }

        public async Task<DOM.ReglaTiempoExtra> CrearAsync(DOM.ReglaTiempoExtra regla)
        {
            var entidad = _mapper.aEntidad(regla);
            _context.ReglaTiempoExtras.Add(entidad);
            await _context.SaveChangesAsync();
            return _mapper.aDominio(entidad);
        }

        public async Task<DOM.ReglaTiempoExtra> ActualizarAsync(DOM.ReglaTiempoExtra regla)
        {
            var entidad = _mapper.aEntidad(regla);
            _context.ReglaTiempoExtras.Update(entidad);
            await _context.SaveChangesAsync();
            return _mapper.aDominio(entidad);
        }

        public async Task EliminarAsync(int id)
        {
            var entidad = await _context.ReglaTiempoExtras.FindAsync(id);
            if (entidad != null)
            {
                _context.ReglaTiempoExtras.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }
    }
}
