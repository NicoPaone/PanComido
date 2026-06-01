using Microsoft.EntityFrameworkCore;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Infraestructura.Persistencia.Mappers;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

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
            int estadoEnviadoId = await _ctx.EstadoPedidos
                .Where(e => e.Descripcion == "Enviado")
                .Select(e => e.Id)
                .FirstAsync();
            efPedido.EstadoPedidoId = estadoEnviadoId;

            await _ctx.SaveChangesAsync();

            return await ObtenerPedidoPorIdAsync(pedidoId);
        }

        public async Task<DOM.Pedido> CrearPedidoAsync(DOM.Pedido pedido)
        {
            int estadoPendienteId = await _ctx.EstadoPedidos
                .Where(e => e.Descripcion == "Pendiente")
                .Select(e => e.Id)
                .FirstAsync();

            EF.Pedido efPedido = _mapper.paraEntidad(pedido, estadoPendienteId);

            _ctx.Pedidos.Add(efPedido);
            await _ctx.SaveChangesAsync();

            EF.Pedido pedidoCompleto = await _ctx.Pedidos
                .Where(p => p.Id == efPedido.Id)
                .Include(p => p.EstadoPedido)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.IdArticuloNavigation)
                .Include(p => p.Proveedor)
                .Include(p => p.PedidoInsumos)
                         .ThenInclude(pi => pi.Insumo)
                         .ThenInclude(i => i.UnidadMedida)
                .FirstAsync();

            return _mapper.paraDominio(pedidoCompleto);

        }

        public async Task<DateOnly?> ObtenerFechaUltimoPedidoDeProveedorAsync(int proveedorId)
        {
            DateOnly? fechaMaxima = await _ctx.Pedidos
                .Where(p => p.ProveedorId == proveedorId)
                .OrderByDescending(p => p.Fecha)
                .Select(p => (DateOnly?)p.Fecha)
                .FirstOrDefaultAsync();

            return fechaMaxima;
        }

        public async Task<DOM.Pedido> ObtenerPedidoPorIdAsync(int pedidoId)
        {
            var efPedido = await _ctx.Pedidos
                .Where(p => p.Id == pedidoId)
                .Include(p => p.EstadoPedido)
                .Include(p => p.Proveedor)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.IdArticuloNavigation)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.UnidadMedida)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.CategoriaInsumo)
                .FirstOrDefaultAsync();
            if (efPedido == null) return null;
            return _mapper.paraDominio(efPedido);
        }

        public async Task<List<DOM.Pedido>> ObtenerPedidosPorProveedorAsync(int proveedorId)
        {
            var efPedidos = await _ctx.Pedidos
                .Where(p => p.ProveedorId == proveedorId)
                .Include(p => p.EstadoPedido)
                .Include(p => p.Proveedor)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.IdArticuloNavigation)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.UnidadMedida)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return efPedidos.Select(_mapper.paraDominio).ToList();
        }

        public async Task<decimal> ObtenerUltimoPrecioCompraUnitarioAsync(int insumoId, int proveedorId)
        {
            var efPrecio = await _ctx.PedidoInsumos
                .Where(pi => pi.InsumoId == insumoId && pi.Pedido.ProveedorId == proveedorId)
                .OrderByDescending(pi => pi.Pedido.Fecha)
                .Select(pi => (decimal?)pi.PrecioCompra)
                .FirstOrDefaultAsync();

            return efPrecio ?? 0;
        }

        public async Task MarcarComoRecibidoAsync(int pedidoId)
        {
            var efPedido = await _ctx.Pedidos
                 .Include(p => p.PedidoInsumos)
                 .FirstOrDefaultAsync(p => p.Id == pedidoId);

            int estadoEnviadoId = await _ctx.EstadoPedidos
                .Where(e => e.Descripcion == "Recibido")
                .Select(e => e.Id)
                .FirstAsync();
            efPedido.EstadoPedidoId = estadoEnviadoId;
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<int>> ObtenerInsumosEnPedidosNoRecibidosAsync(int proveedorId)
        {
            var insumosIds = await _ctx.PedidoInsumos
                .Include(pi => pi.Pedido)
                    .ThenInclude(p => p.EstadoPedido)
                .Where(pi => pi.Pedido.ProveedorId == proveedorId)
                .Where(pi => pi.Pedido.EstadoPedido.Descripcion == "Pendiente" || pi.Pedido.EstadoPedido.Descripcion == "Enviado")
                .Select(pi => pi.InsumoId)
                .Distinct()
                .ToListAsync();

            return insumosIds;

        }
    }
}
