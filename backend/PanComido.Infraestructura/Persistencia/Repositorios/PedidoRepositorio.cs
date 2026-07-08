using Microsoft.EntityFrameworkCore;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class PedidoRepositorio : IPedidoRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly PedidoEntityMapper _mapper;
        public PedidoRepositorio(AppDbContext context, PedidoEntityMapper mapper)
        {
            _ctx = context;
            _mapper = mapper;
        }

        public async Task<DOM.Pedido> EnviarPedidoAsync(int pedidoId, List<DOM.PedidoInsumo> itemsNuevos)
        {
            var efPedido = await _ctx.Pedidos
                .Include(p => p.PedidoInsumos)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            _ctx.PedidoInsumos.RemoveRange(efPedido.PedidoInsumos);

            //agregar nuevos items
            var nuevosItems = itemsNuevos.Select(i => new EF.PedidoInsumo
            {
                PedidoId = pedidoId,
                InsumoId = i.InsumoId,
                Cantidad = i.Cantidad,
                PrecioCompra = i.PrecioCompra
            }).ToList();
            _ctx.PedidoInsumos.AddRange(nuevosItems);

            //cambiar estado a enviado
            efPedido.EstadoPedidoId = (int)EstadoPedido.Enviado;

            await _ctx.SaveChangesAsync();

            return await ObtenerPedidoPorIdAsync(pedidoId);
        }

        public async Task<DOM.Pedido> CrearPedidoAsync(DOM.Pedido pedido)
        {
            EF.Pedido efPedido = _mapper.paraEntidad(pedido, (int)EstadoPedido.Pendiente);

            _ctx.Pedidos.Add(efPedido);
            await _ctx.SaveChangesAsync();

            EF.Pedido pedidoCompleto = await BaseQueryPedido()
                .Where(p => p.Id == efPedido.Id)
                .FirstAsync();

            return _mapper.paraDominio(pedidoCompleto);
        }

        public async Task<DateOnly?> ObtenerFechaUltimoPedidoDeProveedorAsync(int proveedorId)
        {
            DateOnly? fechaMaxima = await _ctx.Pedidos
                .Where(p => p.ProveedorId == proveedorId && !p.Proveedor.Eliminado)
                .OrderByDescending(p => p.Fecha)
                .Select(p => (DateOnly?)p.Fecha)
                .FirstOrDefaultAsync();

            return fechaMaxima;
        }

        public async Task<DOM.Pedido> ObtenerPedidoPorIdAsync(int pedidoId)
        {
            var efPedido = await BaseQueryPedido()
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.CategoriaInsumo)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (efPedido == null) return null;
            return _mapper.paraDominio(efPedido);
        }

        public async Task<List<DOM.Pedido>> ObtenerPedidosPorProveedorAsync(int proveedorId)
        {
            var efPedidos = await BaseQueryPedido()
                .Where(p => p.ProveedorId == proveedorId && !p.Proveedor.Eliminado)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return efPedidos.Select(_mapper.paraDominio).ToList();
        }

        public async Task<decimal> ObtenerUltimoPrecioCompraUnitarioAsync(int insumoId, int proveedorId)
        {
            var efPrecio = await _ctx.PedidoInsumos
                .Where(pi => pi.InsumoId == insumoId
                          && pi.Pedido.ProveedorId == proveedorId
                          && pi.Pedido.EstadoPedidoId == (int)EstadoPedido.Recibido)
                .OrderByDescending(pi => pi.Pedido.Fecha)
                .Select(pi => (decimal?)pi.PrecioCompra)
                .FirstOrDefaultAsync();

            return efPrecio ?? 0;
        }
        public async Task MarcarComoRecibidoAsync(int pedidoId, List<DOM.PedidoInsumo>
        itemsConPrecioConfirmado)
        {
            var efPedido = await _ctx.Pedidos
                 .Include(p => p.PedidoInsumos)
                 .FirstOrDefaultAsync(p => p.Id == pedidoId);

            foreach (var item in itemsConPrecioConfirmado)
            {
                var efItem = efPedido.PedidoInsumos.First(pi => pi.InsumoId == item.InsumoId);
                efItem.PrecioCompra = item.PrecioCompra;
            }

            efPedido.EstadoPedidoId = (int)EstadoPedido.Recibido;
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<int>> ObtenerInsumosEnPedidosNoRecibidosAsync(int proveedorId)
        {
            var insumosIds = await _ctx.PedidoInsumos
                .Include(pi => pi.Pedido)
                    .ThenInclude(p => p.EstadoPedido)
                .Include(pi => pi.Pedido)
                    .ThenInclude(p => p.Proveedor)
                .Where(pi => pi.Pedido.ProveedorId == proveedorId && !pi.Pedido.Proveedor.Eliminado)
                .Where(pi => pi.Pedido.EstadoPedidoId == (int)EstadoPedido.Pendiente || pi.Pedido.EstadoPedidoId == (int)EstadoPedido.Enviado)
                .Select(pi => pi.InsumoId)
                .Distinct()
                .ToListAsync();

            return insumosIds;
        }

        private IQueryable<EF.Pedido> BaseQueryPedido()
        {
            return _ctx.Pedidos
                .Include(p => p.EstadoPedido)
                .Include(p => p.Proveedor)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.IdArticuloNavigation)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.UnidadMedida);
        }
    }
}
