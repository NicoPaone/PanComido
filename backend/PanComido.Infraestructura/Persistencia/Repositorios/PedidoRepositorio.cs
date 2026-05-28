using Microsoft.EntityFrameworkCore;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Infraestructura.Persistencia.Mappers;

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

        public async Task<DateOnly?> ObtenerFechaUltimoPedidoDeProveedorAsync(int proveedorId)
        {
            DateOnly? fechaMaxima = await _ctx.Pedidos
                .Where(p => p.ProveedorId == proveedorId)
                .OrderByDescending(p => p.Fecha)
                .Select(p => (DateOnly?)p.Fecha)
                .FirstOrDefaultAsync();

            return fechaMaxima;
        }

        public async Task<List<DOM.Pedido>> ObtenerPedidosPorProveedorAsync(int proveedorId)
        {
            var efPedidos = await _ctx.Pedidos
                .Where(p => p.ProveedorId == proveedorId)
                .Include(p => p.EstadoPedido)
                .Include(p => p.PedidoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                        .ThenInclude(i => i.IdArticuloNavigation)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return efPedidos.Select(_mapper.paraDominio).ToList();
        }
    }
}
