using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Infraestructura.Persistencia.Mappers;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class FilaVirtualRepositorio : IFilaVirtualRepositorio
    {

        private readonly AppDbContext _ctx;
        private readonly FilaVirtualEntityMapper _filaVirtualEntityMapper;

        public FilaVirtualRepositorio(AppDbContext context, FilaVirtualEntityMapper filaVirtualEntityMapper)
        {
            _ctx = context;
            _filaVirtualEntityMapper = filaVirtualEntityMapper;
        }

        public async Task<FilaVirtual?> ObtenerFilaVirtualAsync(int restauranteId)
        {
            var efFila = await _ctx.FilaVirtuals
                .FirstOrDefaultAsync(f => f.RestauranteId == restauranteId);

            if (efFila == null) return null;

            return _filaVirtualEntityMapper.paraDominio(efFila);
        }

        public async Task<FilaVirtual> ActualizarFilaVirtualAsync(int restauranteId, bool habilitada)
        {
            var efFila = await _ctx.FilaVirtuals
                .FirstOrDefaultAsync(f => f.RestauranteId == restauranteId);

            if (efFila == null)
            {
                efFila = new EF.FilaVirtual { RestauranteId = restauranteId, Habilitada = habilitada };
                await _ctx.FilaVirtuals.AddAsync(efFila);
            }
            else
            {
                _filaVirtualEntityMapper.paraActualizarEntidad(efFila, new FilaVirtual { Habilitada = habilitada });
            }

            await _ctx.SaveChangesAsync();

            return _filaVirtualEntityMapper.paraDominio(efFila);
        }
    }
}
