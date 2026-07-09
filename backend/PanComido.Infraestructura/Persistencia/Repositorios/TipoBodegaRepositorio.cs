using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class TipoBodegaRepositorio : ITipoBodegaRepositorio
    {
        private readonly AppDbContext _ctx;
        
        public TipoBodegaRepositorio(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<DOM.TipoBodega>> ObtenerTodosAsync()
        {
            var tiposEF = await _ctx.TipoBodegas.ToListAsync();
            return tiposEF.Select(t => new DOM.TipoBodega 
            { 
                Id = t.Id, 
                Descripcion = t.Descripcion 
            }).ToList();
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _ctx.TipoBodegas.AnyAsync(t => t.Id == id);
        }
    }
}
