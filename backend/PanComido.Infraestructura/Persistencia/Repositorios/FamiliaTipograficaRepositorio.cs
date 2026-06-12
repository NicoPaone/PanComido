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
    public class FamiliaTipograficaRepositorio : IFamiliaTipograficaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly FamiliaTipograficaEntityMapper _familiaTipograficaEntityMapper;

        public FamiliaTipograficaRepositorio(AppDbContext context, FamiliaTipograficaEntityMapper familiaTipograficaEntityMapper)
        {
            _ctx = context;
            _familiaTipograficaEntityMapper = familiaTipograficaEntityMapper;
        }
        public async Task<List<FamiliaTipografica>> ListarTipografias()
        {
            var efLista = await _ctx.FamiliaTipograficas.ToListAsync();
            return efLista.Select(_familiaTipograficaEntityMapper.paraDominio).ToList();
        }
    }
}
