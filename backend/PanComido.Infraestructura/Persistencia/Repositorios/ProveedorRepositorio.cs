using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class ProveedorRepositorio : IProveedorRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly ProveedorEntityMapper _mapper;

        public ProveedorRepositorio(AppDbContext context, ProveedorEntityMapper mapper)
        {
            _ctx = context;
            _mapper = mapper;
        }

        private IQueryable<EF.Proveedor> BaseQuery(int restauranteId) => _ctx.Proveedors
            .Where(p => p.RestauranteId == restauranteId && !p.Eliminado)
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
               .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);

            return efProveedor == null ? null : _mapper.paraDominio(efProveedor);
        }

        public async Task<DOM.Proveedor> CrearProveedorAsync(DOM.Proveedor proveedor)
        {
            EF.Proveedor efProveedor = _mapper.paraEntidad(proveedor);

            var categorias = await _ctx.CategoriaInsumos
               .Where(c => proveedor.CategoriaIds.Contains(c.Id))
               .ToListAsync();

            efProveedor.CategoriaInsumos = categorias;

            await _ctx.Proveedors.AddAsync(efProveedor);
            await _ctx.SaveChangesAsync();
            return _mapper.paraDominio(efProveedor);
        }

        public async Task<DOM.Proveedor> ModificarProveedorAsync(DOM.Proveedor proveedor)
        {
            EF.Proveedor? efProveedor = await _ctx.Proveedors
                .Include(p => p.CategoriaInsumos)
                .FirstOrDefaultAsync(p => p.Id == proveedor.Id);

            if (efProveedor == null) return null;

            efProveedor.Nombre = proveedor.Nombre;
            efProveedor.NumeroTelefonoWsp = proveedor.NumeroTelefonoWsp;

            var categorias = await _ctx.CategoriaInsumos
                .Where(c => proveedor.CategoriaIds.Contains(c.Id))
                .ToListAsync();

            efProveedor.CategoriaInsumos.Clear();
            efProveedor.CategoriaInsumos = categorias;

            await _ctx.SaveChangesAsync();

            return _mapper.paraDominio(efProveedor);
        }

        public async Task EliminarProveedorAsync(int id)
        {
            EF.Proveedor? efProveedor = await _ctx.Proveedors
                .FirstOrDefaultAsync(p => p.Id == id);

            if (efProveedor == null) return;

            efProveedor.Eliminado = true;
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> ExisteProveedorConNombreAsync(int restauranteId, string nombre)
        {
            return await _ctx.Proveedors
                .AnyAsync(p => p.Nombre == nombre
                       && p.RestauranteId == restauranteId
                       && !p.Eliminado);
        }
    }
}
