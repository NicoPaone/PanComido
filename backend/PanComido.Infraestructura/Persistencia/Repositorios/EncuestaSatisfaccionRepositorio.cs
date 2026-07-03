using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class EncuestaSatisfaccionRepositorio : IEncuestaSatisfaccionRepositorio
    {
        private readonly AppDbContext _context;
        public EncuestaSatisfaccionRepositorio(AppDbContext context)
        {
            _context = context;
        }
        public async Task GuardarEncuestaAsync(DOM.EncuestaSatisfaccion encuestaDominio)
        {
            var efEncuesta = new EF.EncuestaSatisfaccion
            {
                ComandaId = encuestaDominio.ComandaId,
                PuntuacionLugar = encuestaDominio.PuntuacionLugar,
                PuntuacionComida = encuestaDominio.PuntuacionComida,
                PuntuacionMozo = encuestaDominio.PuntuacionMozo,
                Fecha = encuestaDominio.Fecha
            };
            await _context.EncuestaSatisfaccions.AddAsync(efEncuesta);
            await _context.SaveChangesAsync();
        }
    }
}
