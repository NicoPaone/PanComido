using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;


namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class DatosTransferenciaRepositorio : IDatosTransferenciaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly DatosTransferenciaEntityMapper _datosTransferenciaEntityMapper;

        public DatosTransferenciaRepositorio(AppDbContext ctx, DatosTransferenciaEntityMapper datosTransferenciaEntityMapper)
        {
            _ctx = ctx;
            _datosTransferenciaEntityMapper = datosTransferenciaEntityMapper;
        }
        public async Task<DatosTransferencia?> ObtenerDatosTransferenciaAsync(int restauranteId)
        {
            var efDatosTransferencia = await _ctx.DatosTransferencia
                 .Where(dt => dt.RestauranteId == restauranteId)
                 .FirstOrDefaultAsync();

            return efDatosTransferencia != null ? _datosTransferenciaEntityMapper.paraDominio(efDatosTransferencia) : null;
        }

        public async Task<DatosTransferencia> ActualizarDatosTransferenciaAsync(int restauranteId, DatosTransferencia datosTransferencia)
        {
            {
                var efDatosTransferencia = await _ctx.DatosTransferencia
                    .Where(t => t.RestauranteId == restauranteId)
                    .FirstOrDefaultAsync();

                if (efDatosTransferencia == null)
                {
                    efDatosTransferencia = new EF.DatosTransferencium
                    {
                        RestauranteId = restauranteId,
                        Alias = datosTransferencia.Alias,
                        Cbu = datosTransferencia.Cbu,
                        NumeroCuenta = datosTransferencia.NumeroCuenta,
                        TitularCuenta = datosTransferencia.TitularCuenta
                    };
                    await _ctx.DatosTransferencia.AddAsync(efDatosTransferencia);
                }
                else
                {
                    _datosTransferenciaEntityMapper.ActualizarEntidad(efDatosTransferencia, datosTransferencia);
                }

                await _ctx.SaveChangesAsync();
                return efDatosTransferencia != null ? _datosTransferenciaEntityMapper.paraDominio(efDatosTransferencia) : null;
            }
        }
    }
}
