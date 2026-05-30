using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using PanComido.Infraestructura.Persistencia.Mappers;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class UnidadMedidaRepositorio : IUnidadMedidaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly UnidadMedidaEntityMapper _mapper;

        public UnidadMedidaRepositorio(AppDbContext ctx, UnidadMedidaEntityMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<bool> ExisteAsync(int unidadMedidaId)
        {
            return await _ctx.UnidadMedida.AnyAsync(um => um.Id == (int)unidadMedidaId);
        }

        public async Task<List<UnidadMedida>> ObtenerUnidadesDeMedidaAsync()
        {
            List<EF.UnidadMedidum> unidadesDeMedidaEF = await _ctx.UnidadMedida.ToListAsync(); 
            
            return unidadesDeMedidaEF.Select(u => _mapper.paraDominio(u)).ToList();
        }
    }
}
