using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using DOM = PanComido.Dominio.Entidades;


namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class ProveedorRespositorio : IProveedorRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly ProveedorEntityMapper _mapper;

        public ProveedorRespositorio(AppDbContext context, ProveedorEntityMapper mapper)
        {
            _ctx = context;
            _mapper = mapper;
        }

        private IQueryable<EF.Proveedor> BaseQuery(int restauranteId) => _ctx.Proveedors
            .Where(p => p.RestauranteId == restauranteId)
            .Include(p => p.CategoriaInsumos);

        public async Task<List<DOM.Proveedor>> ObtenerProveedoresAsync(int restauranteId)
        {
            List<EF.Proveedor> efLista = await BaseQuery(restauranteId).ToListAsync();
            return efLista.Select(p => _mapper.paraDominio(p)).ToList();
        }

        public async Task<DOM.Proveedor?> ObtenerProveedorPorIdAsync(int id)
        {
            var efProveedor = await _ctx.Proveedors
               .Include(p => p.CategoriaInsumos)
               .FirstOrDefaultAsync(p => p.Id == id);

            return efProveedor == null ? null : _mapper.paraDominio(efProveedor);
        }
    }
}
