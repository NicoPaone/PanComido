using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<FilaVirtual> ObtenerFilaVirtualAsync(int restauranteId)
        {
            var efFila = await _ctx.FilaVirtuals
                .FirstOrDefaultAsync(f => f.RestauranteId == restauranteId);

            if (efFila == null)
                throw new KeyNotFoundException("Fila virtual no encontrada");

            return _filaVirtualEntityMapper.paraDominio(efFila);
        }

        public async Task<FilaVirtual> ActualizarFilaVirtualAsync(int restauranteId, bool habilitada)
        {
            var efFila = await _ctx.FilaVirtuals
                .FirstOrDefaultAsync(f => f.RestauranteId == restauranteId);

            if (efFila == null)
                throw new KeyNotFoundException("Fila virtual no encontrada");

            _filaVirtualEntityMapper.paraActualizarEntidad(efFila, new FilaVirtual { Habilitada = habilitada });
            await _ctx.SaveChangesAsync();

            return _filaVirtualEntityMapper.paraDominio(efFila);
        }
    }
}
