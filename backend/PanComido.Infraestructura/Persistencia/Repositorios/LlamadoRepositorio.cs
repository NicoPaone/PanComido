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
    public class LlamadoRepositorio : ILlamadoRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly LlamadoEntityMapper _llamadoMapper;

        public LlamadoRepositorio(AppDbContext ctx, LlamadoEntityMapper llamadoMapper)
        {
            _ctx = ctx;
            _llamadoMapper = llamadoMapper;
        }

        public Task crearLlamadoAsync(Llamado llamado)
        {
            return Task.CompletedTask;
        }
    }
}
