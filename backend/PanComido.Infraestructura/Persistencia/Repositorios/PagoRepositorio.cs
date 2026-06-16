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
                ComandaId = pago.ComandaId,
                MetodoPagoId = (int)pago.MetodoDePago,
                EstadoPagoId = (int)pago.EstadoPago,
                Total = pago.Total,
                ExternalReference = pago.ExternalReference,
                CierreId = null
            };
            await _ctx.Pagos.AddAsync(efPago);
            await _ctx.SaveChangesAsync();

            pago.PagoId = efPago.Id;
            return pago;
        }
    }
}
