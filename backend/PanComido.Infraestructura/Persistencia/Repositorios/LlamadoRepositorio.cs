using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class LlamadoRepositorio : ILlamadoRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly LlamadoEntityMapper _llamadoMapper;

        public LlamadoRepositorio(AppDbContext ctx, LlamadoEntityMapper llamadoMapper)
        {
            _ctx = ctx;
            _llamadoMapper = llamadoMapper;
        }

        public async Task crearLlamadoAsync(DOM.Llamado llamado)
        {
            EF.Llamado efLlamado = _llamadoMapper.paraEntidad(llamado);
            await _ctx.Llamados.AddAsync(efLlamado);

            await _ctx.SaveChangesAsync();
        }
    }
}
