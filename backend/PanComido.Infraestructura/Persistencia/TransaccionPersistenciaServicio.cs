using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Infraestructura.Persistencia
{
    public class TransaccionPersistenciaServicio : ITransaccionPersistenciaServicio
    {
        private readonly AppDbContext _ctx;

        public TransaccionPersistenciaServicio(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task EjecutarAsync(Func<Task> operacion)
        {
            await using var transaction = await _ctx.Database.BeginTransactionAsync();

            try
            {
                await operacion();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
