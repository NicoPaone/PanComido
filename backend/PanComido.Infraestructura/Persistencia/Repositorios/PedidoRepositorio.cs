using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class PedidoRepositorio : IPedidoRepositorio
    {
        private readonly AppDbContext _ctx;

        public PedidoRepositorio(AppDbContext context)
        {
            _ctx = context;
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
    }
}
