using Microsoft.EntityFrameworkCore;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class PagoRepositorio : IPagoRepositorio
    {
        private readonly AppDbContext _ctx;

        public PagoRepositorio(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<DOM.Pago> CrearPagoAsync(DOM.Pago pago)
        {
            var efPago = new EF.Pago
            {
                MetodoPagoId = pago.MetodoPagoId,
                Total = pago.Total,
                CierreId = null
            };
            await _ctx.Pagos.AddAsync(efPago);
            await _ctx.SaveChangesAsync();

            var efPagoCompleto = await _ctx.Pagos
                .Include(p => p.MetodoPago)
                .FirstAsync(p => p.Id == efPago.Id);

            pago.PagoId = efPagoCompleto.Id;
            pago.MetodoPagoDescripcion = efPagoCompleto.MetodoPago.Descripcion;
            return pago;
        }
    }
}
