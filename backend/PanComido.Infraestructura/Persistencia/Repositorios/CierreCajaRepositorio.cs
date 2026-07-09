using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class CierreCajaRepositorio : ICierreCajaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly CierreCajaEntityMapper _cierreCajaEntityMapper;

        public CierreCajaRepositorio(AppDbContext ctx, CierreCajaEntityMapper cierreCajaEntityMapper)
        {
            _ctx = ctx;
            _cierreCajaEntityMapper = cierreCajaEntityMapper;
        }

        public async Task<DOM.Cierre> CrearCierreDeCajaAsync(DOM.Cierre cierre, List<int> pagoIds)
        {
            var efCierre = _cierreCajaEntityMapper.paraEntidad(cierre);
            await _ctx.Cierres.AddAsync(efCierre);

            var pagos = await _ctx.Pagos
                .Where(p => pagoIds.Contains(p.Id))
                .ToListAsync();

            foreach (var pago in pagos)
            {
                pago.Cierre = efCierre;
            }

            await _ctx.SaveChangesAsync();

            cierre.CierreId = efCierre.Id;
            return cierre;
        }

        public async Task<List<DOM.Cierre>> ObtenerCierresDeCajaAsync(int restauranteId)
        {
            var efCierres = await _ctx.Cierres
                                       .Where(c => c.RestauranteId == restauranteId)
                                       .OrderByDescending(c => c.Fecha)
                                       .ToListAsync();

            return efCierres
                .Select(c => _cierreCajaEntityMapper.paraDominio(c))
                .ToList();
        }
    }
}
